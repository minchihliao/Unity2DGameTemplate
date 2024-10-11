using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameFlowResultState : BaseState<GameFlowController, GameFlowState>
{
    public override async UniTask Enter()
    {
        Debug.Log("Entering result state...");
        await GetController().Transition(GameFlowState.End);
        // 在這裡添加進入結果狀態的邏輯
    }

    public override  UniTask Exit()
    {
        // 在這裡添加離開結果狀態的邏輯
        return default;
    }

    public override void Update()
    {
        // 在這裡添加結果狀態的更新邏輯
    }
}
