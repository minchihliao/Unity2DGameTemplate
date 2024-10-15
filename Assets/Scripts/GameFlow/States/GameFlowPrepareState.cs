using Cysharp.Threading.Tasks;
using UnityEngine;
public class GameFlowPrepareState : BaseState<GameFlowController, GameFlowState>
{
    public override UniTask Enter()
    {
        Debug.Log("Entering prepare state...");
        GetController().Transition(GameFlowState.Play).Forget();
        return default;
        // 在這裡添加進入準備狀態的邏輯
    }

    public override UniTask Exit()
    {
        // 在這裡添加離開準備狀態的邏輯
        return default;
    }

    public override void Update(float deltaTime)
    {
        // 在這裡添加準備狀態的更新邏輯
    }
}