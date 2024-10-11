using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameFlowEndState : BaseState<GameFlowController, GameFlowState>
{
    [Inject]
    private MainFlowController mainFlowController;


    public override UniTask Enter()
    {
        Debug.Log("Entering GameFlow End State");
        return default;
    }

    public override UniTask Exit()
    {
        // 在這裡添加離開遊戲結束狀態的邏輯
        Debug.Log("Exiting GameFlow End State");
        return default;
    }

    public override void Update()
    {
        // 在這裡添加遊戲結束狀態的更新邏輯
        // 例如：检查是否需要返回主菜单或重新开始游戏
        if (Input.GetKeyDown(KeyCode.R))
        {
            mainFlowController.Transition(MainFlowState.Menu).Forget();
        }
         if (Input.GetKeyDown(KeyCode.Escape))
        {
            mainFlowController.Transition(MainFlowState.End).Forget();
        }
    }
}