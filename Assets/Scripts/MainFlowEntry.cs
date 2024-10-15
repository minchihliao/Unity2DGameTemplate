using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

public class MainFlowEntry : MonoBehaviour
{
    [Inject] private MainFlowController mainFlowController;
    public static MainFlowEntry instance;
    void Awake()
    {
        if (instance == null) instance = this;
    }
    private void OnDestroy()
    {
        mainFlowController.Dispose();
    }

    private async void Start()
    {
        await mainFlowController.Transition(MainFlowState.Init);
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        mainFlowController.Update(deltaTime);
    }

}