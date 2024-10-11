using UnityEngine;
using UniRx;
using Cysharp.Threading.Tasks;
using Zenject;
using System;

public class UIManager :MonoBehaviour, IInitializable, IDisposable
{
    [Inject] private MainFlowController _mainFlowController;
    [Inject] private GameFlowController _gameFlowController;

    private CompositeDisposable _disposables = new CompositeDisposable();

    // UI元素的公共引用
    public GameObject menuUI;
    public GameObject gameUI;
    public GameObject resultUI;

    public void Initialize()
    {
        // 訂閱MainFlowController的狀態變化
        _mainFlowController.GetCurrentStateObservable()
            .Subscribe(OnMainFlowStateChanged)
            .AddTo(_disposables);

        // 訂閱GameFlowController的狀態變化
        _gameFlowController.GetCurrentStateObservable()
            .Subscribe(OnGameFlowStateChanged)
            .AddTo(_disposables);
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }

    private void OnMainFlowStateChanged(IState<MainFlowController, MainFlowState> state)
    {
        switch (state)
        {
            case MainFlowMenuState:
                ShowMenuUI();
                break;
            case MainFlowGameState:
                ShowGameUI();
                break;
            default:
                HideAllUI();
                break;
        }
    }

    private void OnGameFlowStateChanged(IState<GameFlowController, GameFlowState> state)
    {
        // 根據GameFlow狀態更新遊戲UI
        // 這裡可以添加更多的邏輯
    }

    private void ShowMenuUI()
    {
        menuUI.SetActive(true);
        gameUI.SetActive(false);
        resultUI.SetActive(false);
    }

    private void ShowGameUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(true);
        resultUI.SetActive(false);
    }

    private void ShowResultUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(false);
        resultUI.SetActive(true);
    }

    private void HideAllUI()
    {
        menuUI.SetActive(false);
        gameUI.SetActive(false);
        resultUI.SetActive(false);
    }

    // 可以添加更多的方法來更新UI元素
    public async UniTask UpdateScoreAsync(int score)
    {
        // 更新分數UI的異步方法
        await UniTask.SwitchToMainThread();
        // 更新UI邏輯
    }

    public IObservable<Unit> OnStartButtonClicked()
    {
        // 返回一個可觀察的序列，當開始按鈕被點擊時發出信號
        return Observable.FromEvent(
            h => menuUI.GetComponentInChildren<UnityEngine.UI.Button>().onClick.AddListener(() => h()),
            h => menuUI.GetComponentInChildren<UnityEngine.UI.Button>().onClick.RemoveListener(() => h())
        );
    }
}
