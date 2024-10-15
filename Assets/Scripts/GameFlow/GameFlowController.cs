using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class GameFlowController : BaseControl<GameFlowController, GameFlowState>
{
    public override void Initialize()
    {
        base.Initialize();

        RegisterState(GameFlowState.Prepare, new GameFlowPrepareState());
        RegisterState(GameFlowState.Play, new GameFlowPlayState());
        RegisterState(GameFlowState.End, new GameFlowEndState());
        RegisterState(GameFlowState.Result, new GameFlowResultState()); // 新增 Result state
    }

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        // 在這裡可以添加特定於GameFlowController的更新邏輯
    }
}