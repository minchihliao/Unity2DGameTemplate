using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // 綁定 流程相關的Controller
        Container.BindInterfacesAndSelfTo<MainFlowController>().AsSingle().NonLazy();;
        Container.BindInterfacesAndSelfTo<GameFlowController>().AsSingle().NonLazy();;
        Container.Bind<MainFlowEntry>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

        // 綁定 UIManager
        Container.Bind<UIManager>().FromComponentInHierarchy().AsSingle();


        // 綁定 MainFlowEntry
        

        // 其他綁定...
    }
}