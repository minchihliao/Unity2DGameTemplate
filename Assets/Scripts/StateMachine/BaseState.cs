using UnityEngine;
using Cysharp.Threading.Tasks;

public interface IState<T, U>
{
    UniTask Enter();
    UniTask Exit();
    void Update();
    void SetController(T controller);
}

public abstract class BaseState<T, U> : IState<T, U>
{
    protected T Controller { get; private set; }

    public abstract UniTask Enter();
    public abstract UniTask Exit();

    public virtual void Update() { }

    public void SetController(T controller)
    {
        Controller = controller;
    }

    public T GetController()
    {
        return Controller;
    }
}