using Cysharp.Threading.Tasks;
using UnityEngine;
public class GameFlowPlayState : BaseState<GameFlowController, GameFlowState>
{
    public override async UniTask Enter()
    {
        Debug.Log("Entering play state...");
        await GetController().Transition(GameFlowState.Result);
        // 在這裡添加進入遊戲進行狀態的邏輯
    }

    public override UniTask Exit()
    {
        // 在這裡添加離開遊戲進行狀態的邏輯
        return default;
    }

    public override void Update(float deltaTime)
    {
        // 在這裡添加遊戲進行狀態的更新邏輯
    }
}