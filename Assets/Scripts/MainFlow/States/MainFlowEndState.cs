using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainFlowEndState : BaseState<MainFlowController, MainFlowState>
{


     public override  UniTask Enter()
    {
      Debug.Log("Entering MainFlow End State");
      return default;

    }

    public override  UniTask Exit()
    {
        Debug.Log("Exiting MainFlow End State");
        return default;
        // 在这里添加退出 End 状态时的逻辑
        // 例如：清理游戏结束界面
    }

    public override void Update(float deltaTime)
    {
        // 在这里添加 End 状态的更新逻辑

    }
}