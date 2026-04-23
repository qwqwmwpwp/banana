using System;
using UnityEngine;
using EnemyCommon;

public partial class BlackYellowFireController : EnemyCommonController
{
    public int damage = 100;
    protected override void Awake()
    {
        base.Awake();
        stateMachine.Init(this);
    }
    
    public void Start()
    {
        bodyCollider = GetComponent<Collider2D>();
        ChangeState(EnemyCommonStateType.Patrol);
    }
  

    public virtual void ChangeState(EnemyCommonStateType state)
    {
        currentState = state;

        switch(state)
        {
            case EnemyCommonStateType.Patrol:
                stateMachine.SwitchState<BlackYellowFirePatrolState>();
                break;
            case EnemyCommonStateType.Chase:
                stateMachine.SwitchState<BlackYellowFireChaseState>();
                break;
            default:
                stateMachine.SwitchState<BlackYellowFireStateBase>();
                break;

        }
    }

    private void OnDestroy()
    {
        stateMachine.Clear();
    }

    public virtual void ResetSelf()
    {
        transform.position = respawnPoint.position;
    }

    public override void Init(EnemySpawnContext ctx)
    {
       base.Init(ctx);
        ChangeState(EnemyCommonStateType.Patrol);
    }
}
