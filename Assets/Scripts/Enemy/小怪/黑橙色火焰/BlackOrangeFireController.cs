using EnemyCommon;
using UnityEngine;

public partial class BlackOrangeFireController : EnemyCommonController
{
    public void Start()
    {
        stateMachine.Init(this);
        rb = GetComponent<Rigidbody2D>();
        bodyCollider = GetComponent<Collider2D>();
        ChangeState(EnemyCommonStateType.Patrol);
    }



    public virtual void ChangeState(EnemyCommonStateType state)
    {
        currentState = state;

        switch (state)
        {
            case EnemyCommonStateType.Patrol:
                stateMachine.SwitchState<BlackOrangeFirePatrolState>();
                break;
            case EnemyCommonStateType.Chase:
                stateMachine.SwitchState<BlackOrangeFireChaseState>();
                break;
            default:
                stateMachine.SwitchState<BlackOrangeFireStateBase>();
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
    }
}
