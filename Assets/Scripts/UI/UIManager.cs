using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Zenject;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class UIManager :MonoBehaviour, IInitializable, IDisposable
{

    [Inject]
    AssetManager assetManager;
    [Inject]
    DiContainer diContainer;


    Dictionary<string, GameObject> prefabes = new Dictionary<string, GameObject>();
    private CompositeDisposable _disposables = new CompositeDisposable();
    [SerializeField]
    List<GameObject> OpenedUI = new List<GameObject>();
    string debugColorString = "<color=#006000>{0}</color>";
    [SerializeField]
    RectTransform overlayCanvas;
    [SerializeField]
    GameObject loadingUI;
    [SerializeField]
    Image fadeUI;


    public void Initialize()
    {


    }

    void Awake()
    {
        if (loadingUI != null)
        {
            var loadImg = loadingUI.transform.GetChild(0);
            loadImg.DOLocalRotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }
    }
    async public UniTask FadeIn(float s)
    {
        fadeUI.gameObject.SetActive(true);
        fadeUI.color = Color.clear;
        await fadeUI.DOFade(1f, s).AsyncWaitForCompletion();
    }

    async public UniTask FadeOut(float s)
    {
        fadeUI.gameObject.SetActive(true);
        await fadeUI.DOFade(1f, 0).AsyncWaitForCompletion();
        await fadeUI.DOFade(0f, s).AsyncWaitForCompletion();
        fadeUI.gameObject.SetActive(false);
    }
    public void Dispose()
    {
        _disposables.Dispose();
    }

    async UniTask<GameObject> LoadUIPrefab(string prefabName)
    {   
        if (!prefabes.TryGetValue(prefabName, out GameObject prefab))
        {
            prefab = await assetManager.LoadAssetAsync<GameObject>($"Window/{prefabName}.prefab");
            prefabes.Add(prefabName, prefab);
        }
    return prefab;
    }

    public T FindUI<T>() where T : UIBase
    {
        var uiName = typeof(T).ToString();
        var obj = OpenedUI.Find(o => o.name == uiName);
        T ui = null;
        if (!obj)
        {
            Debug.LogWarning("Find UI not Opened : " + uiName);
        }
        else
        {
            ui = obj.GetComponent<T>();
            if (!ui) Debug.LogWarning("Find UI GameObject But not Script: " + uiName);
        }
        return ui;
    }

    public async UniTask<T> OpenUI<T>(bool showLoading = false) where T : UIBase
    {
        if (showLoading) LoadingUI(true);
        string prefabName = typeof(T).ToString();
        T ui = null;
        try
        {
            GameObject prefab = await LoadUIPrefab(prefabName);
            var obj = GameObject.Instantiate(prefab, overlayCanvas);
            if (obj)
            {
                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.sizeDelta = Vector2.zero;
                obj.name = prefabName;
                OpenedUI.Add(obj);
                ui = obj.GetComponent<T>();
            }
            else Debug.LogWarningFormat("UI 開啟失敗: {0} 不存在或 物件名稱與腳本名稱不同 obj is null", typeof(T).ToString());
            Debug.LogFormat(debugColorString, "開啟 UI : " + prefabName);
            if (ui != null)
            {
                diContainer.InjectGameObjectForComponent<T>(ui.gameObject);
                await ui.OnOpen(); 
            }
            else Debug.LogWarningFormat("UI: {0} 不存在 或 物件名稱與腳本名稱不同 obj name is: {1}", typeof(T).ToString(), obj.name);
            return ui;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            Debug.LogError(ex.StackTrace);
        }
        finally
        {
            if (showLoading) LoadingUI(false);
        }
        return null;
    }

    public void LoadingUI(bool isShow)
    {
        loadingUI.gameObject.SetActive(isShow);
        Debug.LogFormat(debugColorString, "LoadingUI : " + isShow);
    }



}
