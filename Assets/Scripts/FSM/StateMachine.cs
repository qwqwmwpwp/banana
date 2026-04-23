using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    private StateBase currentState; // 当前状态
    public bool HasState => currentState != null; //当前状态是否为空
    public StateBase CurrentState => currentState;    private IStateMachineOwner owner;
    private Dictionary<Type, StateBase> stateDic;

    public void Init(IStateMachineOwner own)
    {
        this.owner = own;
        stateDic = new Dictionary<Type, StateBase>();
    }
    
   /// <summary>
   /// 切换状态
   /// </summary>
   /// <typeparam name="T">实际状态</typeparam>
    public void SwitchState<T>(bool refresh = false) where T : StateBase, new()
    {
        //状态重复
        if (HasState && typeof(T) == currentState.GetType() && !refresh) return;
        //退出上一个状态
        if (currentState != null)
        {
            ExitState();
        }
        //进入状态
        currentState = SetState<T>();
        EnterState();

    }



    private void EnterState()
    {
        currentState.OnEnter();
        MonoManager.Instance.AddUpdate(currentState.OnUpdate);
        MonoManager.Instance.AddFixedUpdate(currentState.OnFixedUpdate);
        MonoManager.Instance.AddLateUpdate(currentState.OnLateUpdate);
    }
    private void ExitState()
    {
        MonoManager.Instance.RemoveUpdate(currentState.OnUpdate);
        MonoManager.Instance.RemoveFixedUpdate(currentState.OnFixedUpdate);
        MonoManager.Instance.RemoveLateUpdate(currentState.OnLateUpdate);
        currentState.OnExit();
    }
    // 加载状态
    private StateBase SetState<T>() where T : StateBase, new()
    {
        if (!stateDic.TryGetValue(typeof(T), out StateBase state))
        {
            state = new T();
            state.Init(owner);
            stateDic.Add(typeof(T), state);
        }

        return state;
    }
    // 清理
    public void Clear()
    {
        if (currentState != null)
        {
            ExitState();
            currentState = null;
        }
        stateDic?.Clear();
    }

}