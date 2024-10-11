using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainFlowMenuState : BaseState<MainFlowController, MainFlowState>
{
    public override  UniTask Enter()
    {
        Debug.Log("Entering main menu...");
        return default;
        // 在這裡顯示主菜單UI
    }

    public override void Update()
    {
        // 檢查是否應該開始遊戲
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space key pressed, transitioning to Game state");
            GetController().Transition(MainFlowState.Game).Forget();
        }
    }

    public override  UniTask Exit()
    {
        return default;
    }
}
