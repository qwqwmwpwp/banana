using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateBase
{
     public abstract void Init(IStateMachineOwner owner);
     public abstract void OnEnter();
     public abstract void OnUpdate();
     public abstract void OnFixedUpdate();
     public abstract void OnLateUpdate();
     public abstract void OnExit();
}
