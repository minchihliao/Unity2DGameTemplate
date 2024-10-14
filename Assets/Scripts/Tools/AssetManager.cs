using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UniRx;
using Zenject;
using UnityEngine.Pool;

public class AssetManager : MonoBehaviour, IInitializable, IDisposable
{
    private readonly Dictionary<string, ObjectPool<GameObject>> _pools = new Dictionary<string, ObjectPool<GameObject>>();
    private readonly CompositeDisposable _disposables = new CompositeDisposable();
    private AsyncOperationHandle<IList<IResourceLocation>> downloadOperation;

    public void Initialize()
    {
        Debug.Log("AssetManager initialized");
    }

    public void Dispose()
    {
        _disposables.Dispose();
        foreach (var pool in _pools.Values)
        {
            pool.Dispose();
        }
        _pools.Clear();
    }

    public async UniTask<T> LoadAssetAsync<T>(string key) where T : UnityEngine.Object
    {
        try
        {
            var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
            await asyncOperationHandle.ToUniTask();
            if (asyncOperationHandle.Status == AsyncOperationStatus.Succeeded)
            {
                return asyncOperationHandle.Result;
            }
            else
            {
                Debug.LogError(asyncOperationHandle.OperationException.Message);
                Debug.LogError(asyncOperationHandle.OperationException.StackTrace);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            Debug.LogError(e.StackTrace);
        }
        return default(T);
    }

    public async UniTask<GameObject> LoadAndInstantiateAsync(string key, Transform parent = null, bool useObjectPool = false)
    {
        if (useObjectPool)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool.Get();
            }
            else
            {
                // 如果池不存在，創建一個新的池
                await CreatePoolAsync(key);
                return _pools[key].Get();
            }
        }
        else
        {
            var prefab = await LoadAssetAsync<GameObject>(key);
            return UnityEngine.Object.Instantiate(prefab, parent);
        }
    }

    public ObjectPool<GameObject> SetDefaultObject(string key, GameObject prefab, int initialSize = 5, int maxSize = 10)
    {
        if (_pools.TryGetValue(key, out var existingPool))
        {
            Debug.LogWarning($"Pool with key '{key}' already exists. Returning existing pool.");
            return existingPool;
        }

        var pool = new ObjectPool<GameObject>(
            createFunc: () => UnityEngine.Object.Instantiate(prefab),
            actionOnGet: (obj) => obj.SetActive(true),
            actionOnRelease: (obj) => obj.SetActive(false),
            actionOnDestroy: (obj) => UnityEngine.Object.Destroy(obj),
            collectionCheck: false,
            defaultCapacity: initialSize,
            maxSize: maxSize
        );

        _pools[key] = pool;
        return pool;
    }

    private async UniTask<ObjectPool<GameObject>> CreatePoolAsync(string key, int initialSize = 5, int maxSize = 10)
    {
        if (_pools.TryGetValue(key, out var existingPool))
        {
            return existingPool;
        }

        var prefab = await LoadAssetAsync<GameObject>(key);
        return SetDefaultObject(key, prefab, initialSize, maxSize);
    }

    public GameObject GetFromPool(string key)
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            return pool.Get();
        }
        Debug.LogWarning($"No pool found for key: {key}");
        return null;
    }
    public List<GameObject> GetManyFromPool(string key, int count)
    {
        var result = new List<GameObject>();
        if (_pools.TryGetValue(key, out var pool))
        {
            for (int i = 0; i < count; i++)
            {
                result.Add(pool.Get());
            }
        }
        return result;
    }


    public void ReleaseToPool(string key, GameObject instance)
    {
        if (_pools.TryGetValue(key, out var pool))
        {
            pool.Release(instance);
        }
        else
        {
            Debug.LogWarning($"No pool found for key: {key}. Destroying the instance instead.");
            UnityEngine.Object.Destroy(instance);
        }
    }

        // 新增一個重載方法，允許直接傳入 GameObject 而不需要 key
    public void ReleaseToPool(GameObject instance)
    {
        foreach (var kvp in _pools)
        {
            if (kvp.Value.CountInactive < kvp.Value.CountAll)
            {
                kvp.Value.Release(instance);
                return;
            }
        }
        
        Debug.LogWarning($"No suitable pool found for the given instance. Destroying it instead.");
        UnityEngine.Object.Destroy(instance);
    }




    public IObservable<float> GetDownloadSizeAsync(IEnumerable<string> keys)
    {
        return Observable.Create<float>(observer =>
        {
            var sizeOperation = Addressables.GetDownloadSizeAsync(keys);
            sizeOperation.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    float sizeInMB = op.Result / (1024f * 1024f);
                    observer.OnNext(sizeInMB);
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnError(new Exception($"獲取下載大小失敗: {op.OperationException}"));
                }
            };

            return Disposable.Create(() =>
            {
                if (!sizeOperation.IsDone)
                {
                    Addressables.Release(sizeOperation);
                }
            });
        });
    }

    public IObservable<float> DownloadDependenciesAsync(IEnumerable<string> keys)
    {
        return Observable.Create<float>(observer =>
        {
            AsyncOperationHandle downloadOperation = Addressables.DownloadDependenciesAsync(keys);
            
            downloadOperation.Completed += op =>
            {
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    observer.OnNext(100f);
                    observer.OnCompleted();
                }
                else
                {
                    observer.OnError(new Exception($"下載失敗: {op.OperationException}"));
                }
            };

            Observable.EveryUpdate()
                .TakeWhile(_ => !downloadOperation.IsDone)
                .Subscribe(_ => observer.OnNext(downloadOperation.PercentComplete * 100f));

            return Disposable.Create(() =>
            {
                if (!downloadOperation.IsDone)
                {
                    Addressables.Release(downloadOperation);
                }
            });
        })
        .DoOnSubscribe(() => Debug.Log("開始下載"))
        .DoOnCompleted(() => Debug.Log("下載完成"))
        .DoOnError(error => Debug.LogError($"下載錯誤: {error}"));
    }



}
