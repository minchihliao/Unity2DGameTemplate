using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class MainFlowGameState : BaseState<MainFlowController, MainFlowState>
{
    [Inject]
    private GameFlowController gameFlowController;
    public override  async UniTask Enter()
    {
        Debug.Log("Starting game...");
        // 在這裡開始遊戲邏輯
        // 可能需要與 GameFlowController 進行交互
        await gameFlowController.Transition(GameFlowState.Prepare);
    }

      public override  UniTask Exit()
    {
        // 在這裡添加離開遊戲狀態的邏輯
        return default;
    }


    public override void Update(float deltaTime)
    {
        gameFlowController.Update(deltaTime);
    }
}