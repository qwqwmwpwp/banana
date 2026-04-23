using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackWhiteFire : EnemyBase, IStateMachineOwner
{
    
    private StateMachine StateMachine = new StateMachine();


    [Header("移动模块")]
    public float moveSpeed = 2f;  // 移动速度
    public float chaseSpeed = 2f; // 追击速度
    public float waitTime = 2f;   // 静止等待的时间

    [SerializeField] private Transform Transform1; // 位置1
    [SerializeField] private Transform Transform2; // 位置2
    private Vector3 position1;
    private Vector3 position2;
    private Vector3 targetPosition; // 目标位置      
    private bool isWaiting = false; // 是否在等待

    [Header("攻击模块")]
    public float SelfDestructingRadius;  //自爆半径

    [Header("索敌范围")]
    public float detectionDistance; //索敌距离
    public float detectionAngle;    //索敌角度
    public LayerMask playerLayer;

    Animator anim;
    protected void Start()
    {
       // stats = GetComponent<EnemyStats>();
        anim = GetComponent<Animator>();
        StateMachine.Init(this);

        // 获取位置
        position1 = Transform1.position;
        position2 = Transform2.position;
        targetPosition = position1;
        ChangeState(EnemyStateType.Partol);

    }

    public void StartPatrol()
    {
        // 启动移动协程
        StartCoroutine(Patrol());

    }
    public void StopPatrol()
    {
        // 关闭移动协程
        StopCoroutine(Patrol());
    }

    public void StartSelfDestructing()
    {
        StartCoroutine(SelfDestructing());
    }

    IEnumerator Patrol()//移动协程
    {
        while (true)
        {
            if (isWaiting)
            {
                // 等待2秒
                yield return new WaitForSeconds(waitTime);

                // 等待结束，开始转身
                isWaiting = false;

                // 如果当前位置是起始位置
                if (Vector3.Distance(transform.position, position1) < 0.5f && targetPosition == position1)
                    targetPosition = position2;
                // 如果当前位置是目标位置
                else if (Vector3.Distance(transform.position, position2) < 0.5f && targetPosition == position2)
                    targetPosition = position1;
            }

            // 移动怪物
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 判断是否到达目标位置
            if (transform.position == targetPosition)
                isWaiting = true;

            // 等待下一帧
            yield return null;
        }
    }
    IEnumerator SelfDestructing()//自爆携程
    {
        yield return new WaitForSeconds(0.5f);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, SelfDestructingRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<PlayerController>() != null)
            {


                //取得攻击对象的 Stats 
                PlayerStats targetStats = hit.GetComponent<PlayerStats>();

                //调用人物受击函数
                targetStats.TakeDamage(stats.damage.GetValue(),AttackType.Melee);

                //预留动画
                anim.SetTrigger("boom");
            }
        }

        
    }
    //销毁自身

    public void Des()
    {
        Dead();

    }

    public void ChangeState(EnemyStateType state)
    {
        switch (state)
        {
            case EnemyStateType.Partol:
                StateMachine.SwitchState<PatrolState_BlackWhiteFire>();
                break;
            case EnemyStateType.Chase:
                StateMachine.SwitchState<ChaseState_BlackWhiteFire>();
                break;
        }
    }
}
