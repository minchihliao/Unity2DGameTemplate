using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainFlowInitState : BaseState<MainFlowController, MainFlowState>
{
    public override async UniTask Enter()
    {
        Debug.Log("Initializing ...");
        // 在這裡進行遊戲初始化操作
        await UniTask.Delay(1000); // 模擬初始化過程
        await GetController().Transition(MainFlowState.Menu);
    }

    public override UniTask Exit()
    {
        return default;
    }
}