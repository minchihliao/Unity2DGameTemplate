using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
public class MainFlowMenuState : BaseState<MainFlowController, MainFlowState>
{
    [Inject]
    UIManager uiManager;
    async public override UniTask Enter()
    {
        Debug.Log("Entering main menu...");
        await uiManager.OpenUI<UIMenu>();
    }

    public override void Update(float deltaTime)
    {
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
