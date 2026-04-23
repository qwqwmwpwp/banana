using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnemyCommon;

public class BlackOrangeFireStateBase : EnemyCommonStateBase
{
    public BlackOrangeFireController controller;
    
    public override void Init(IStateMachineOwner owner)
    {
        controller = owner as BlackOrangeFireController;
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
