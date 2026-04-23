using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWhiteFire_State : StateBase
{
    public BlackWhiteFire enemy;

    public override void Init(IStateMachineOwner owner)
    {
        enemy = owner as BlackWhiteFire;
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
