using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStateType
{
    Idle, 
    Partol, 
    Chase, 
    Charging,
    Dead
}

public class BlackRedFire_Ranged : EnemyBase, IStateMachineOwner
{
    private StateMachine StateMachine = new StateMachine();

    [Header("攻击模块")]
    public fireball fireballPrefab;
    public float fireballCooldown;      //火球攻击冷却
    public float fireballCooldownTimer; //计时器
    public float ChargeTime;            //蓄力时间

    [Header("索敌范围")]
    public float detectionDistance; //索敌距离
    public float detectionAngle;    //索敌角度
    public LayerMask playerLayer;


    protected void Start()
    {
        //stats = GetComponent<EnemyStats>();
        StateMachine.Init(this);
        ChangeState(EnemyStateType.Idle);
    }

    protected void Update()
    {

        fireballCooldownTimer -= Time.deltaTime;

    }

    public void InstantiateFireball()
    {
        //释放火球逻辑
        PlayerController player = PlayerManager.instance.player;
        Vector3 playerPos = player.transform.position;
        Vector3 enemyPos = transform.position;

        // 计算方向向量
        Vector3 direction = (enemyPos - playerPos).normalized;

        // 将方向向量，转换成相对于 X 轴的角度
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 将其转化成四元数
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        //生成火球，朝向 player
        fireball newFireball = Instantiate(fireballPrefab, transform.position, rotation);

        newFireball.Setup(stats.damage.GetValue(), player.playerStats, -direction);
    }

    public void ChangeState(EnemyStateType state)
    {
        switch (state)
        {
            case EnemyStateType.Idle:
                StateMachine.SwitchState<IdleState_BlackRedFire_Ranged>();

                break;
            case EnemyStateType.Charging:
                StateMachine.SwitchState<ChargingState_BlackRedFire_Ranged>();
                break;
        }
    }
}
