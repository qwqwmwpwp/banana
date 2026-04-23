using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EnemyCommon;

public class BlackYellowFireStateBase : EnemyCommonStateBase
{
    public BlackYellowFireController controller;
    
    public override void Init(IStateMachineOwner owner)
    {
        controller = owner as BlackYellowFireController;
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
