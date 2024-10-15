using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class MainFlowController : BaseControl<MainFlowController, MainFlowState>
{
    [Inject] private GameFlowController _gameFlowController;

    public override void Initialize()
    {
        base.Initialize();

        RegisterState(MainFlowState.Init, new MainFlowInitState());
        RegisterState(MainFlowState.Menu, new MainFlowMenuState());
        RegisterState(MainFlowState.Game, new MainFlowGameState());
        RegisterState(MainFlowState.End, new MainFlowEndState());

        Debug.Log("MainFlowController initialized");
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        // 在這裡可以添加特定於MainFlowController的更新邏輯
    }

    public override void Stop()
    {
        base.Stop();
        _gameFlowController.Stop();
    }
}
