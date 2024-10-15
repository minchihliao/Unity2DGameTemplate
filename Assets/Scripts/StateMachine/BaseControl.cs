using System;
using System.Collections.Generic;
using Zenject;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using System.Threading;

public abstract class BaseControl<TController, TState> : IInitializable, IDisposable where TController : BaseControl<TController, TState>
{
    protected ReactiveProperty<IState<TController, TState>> CurrentState = new ReactiveProperty<IState<TController, TState>>();
    protected CompositeDisposable Disposables = new CompositeDisposable();
    protected Dictionary<TState, IState<TController, TState>> States = new Dictionary<TState, IState<TController, TState>>();
    protected CancellationTokenSource Cts = new CancellationTokenSource();

    [Inject] protected DiContainer Container;

    public virtual void Initialize()
    {
        CurrentState.Subscribe(OnStateChanged).AddTo(Disposables);
    }

    public virtual void Dispose()
    {
        if (Cts != null && !Cts.IsCancellationRequested)
        {
            Cts.Cancel();
            Cts.Dispose();
            Cts = null;
        }
        Disposables.Dispose();
    }

    protected virtual void OnStateChanged(IState<TController, TState> state)
    {
        if (state == null)
        {
            Debug.LogWarning("Attempted to change to a null state.");
            return;
        }

        Debug.Log($"State changed to: {state.GetType().Name}");
        // 其他处理逻辑...
    }

    public virtual void Update(float deltaTime)
    {
        if (!Cts.IsCancellationRequested)
        {
            CurrentState.Value?.Update(deltaTime);
        }
    }

    protected void RegisterState(TState condition, IState<TController, TState> state)
    {
        if (!States.ContainsKey(condition))
        {
            States.Add(condition, state);
            state.SetController((TController)this);
            Container.Inject(state);
        }
        else
        {
            Debug.LogWarning($"Duplicate state registration: {condition} for state {state.GetType().Name}");
        }
    }

    public virtual async UniTask Transition(TState condition)
    {
        if (Cts.IsCancellationRequested)
        {
            return;
        }

        if (States.TryGetValue(condition, out var newState))
        {
            if (CurrentState.Value != null)
            {
                await CurrentState.Value.Exit();
            }

            CurrentState.Value = newState;
            await newState.Enter();
        }
        else
        {
            Debug.LogWarning($"No state registered for condition: {condition}");
        }
    }

    public IObservable<IState<TController, TState>> GetCurrentStateObservable()
    {
        return CurrentState;
    }

    public virtual void Stop()
    {
        Cts.Cancel();
        CurrentState.Value?.Exit().Forget();
        CurrentState.Value = null;
        Debug.Log($"{GetType().Name} stopped.");
    }
}
