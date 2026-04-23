using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackRedFire_Ranged_State : StateBase
{
    protected BlackRedFire_Ranged enemy;

    public override void Init(IStateMachineOwner owner)
    {
        enemy = owner as BlackRedFire_Ranged;
    }

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnFixedUpdate()
    {
    }

    public override void OnLateUpdate()
    {
    }

    public override void OnUpdate()
    { 
    }
}
