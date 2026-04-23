using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EliteState : StateBase
{
    public EliteController eliteController;

    public override void Init(IStateMachineOwner owner)
    {
        eliteController = owner as EliteController;
    }

    public override void OnEnter()
    {
        
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnFixedUpdate()
    {
        
    }

    public override void OnLateUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}
