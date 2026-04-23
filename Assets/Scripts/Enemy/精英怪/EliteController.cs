using System.Collections;
using System.Collections.Generic;
using Enemy.精英怪.State;
using Unity.VisualScripting;
using UnityEngine;

public  class EliteController : EnemyBase, IStateMachineOwner
{
    private StateMachine StateMachine = new StateMachine();
    [Header("怪物视野")]
    public float viewRadius = 5f;          // 视野半径
    [Range(0, 360)]
    public float viewAngle = 90f;         // 视野角度（扇形展开角度）
    public LayerMask targetMask;          // 玩家所在层级
        
    [Header("Ranged Attack Settings")]
    public float rangedAttackCooldown = 5f;
    private float rangedAttackCooldownTimer;
    [Header("当前状态")]
    public EliteStateType currentState;
    
    [Header("火球预制体")]
    public GameObject fireballPrefab;
    [Header("火球偏移")]
    public Vector3 offset;

    [Header("当前状态")] public Transform [] patrolPoints;
        [HideInInspector]    public float FacingDirection
        {
            get { return Mathf.Sign(transform.localScale.x); }
            set { transform.localScale = new Vector3(value, 1, 1); }
        }

        public bool IsRangedAttackOnCooldown => rangedAttackCooldownTimer > 0;
  
    [HideInInspector] 
    // public Rigidbody2D rb;
    public Animator aniator;
   
    protected  void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StateMachine.Init(this);
       ChangeState(EliteStateType.ElitePartol);
    }

    public void ChangeState(EliteStateType state)
  {
      currentState = state;
      switch (state)
      {
          case EliteStateType.EliteIdle:
              StateMachine.SwitchState<EliteIdleState>();
              break;
          case EliteStateType.ElitePartol:
               StateMachine.SwitchState<ElitePatrolState>();
              break;
          case EliteStateType.EliteChase:
              StateMachine.SwitchState<EliteChaseState>();
              break;
          case EliteStateType.EliteRemoteAttack:
              StateMachine.SwitchState<EliteRemoteAttackState>();
              break;
          case EliteStateType.EliteCloseAttack:
              StateMachine.SwitchState<EliteCloseAttackState>();
              break;
      }
  }

    protected void Update()
    {
        UpdateCooldowns();
    }
    private void UpdateCooldowns()
    {
        if (rangedAttackCooldownTimer > 0)
        {
            rangedAttackCooldownTimer -= Time.deltaTime;
        }
    }
    
    public void StartRangedAttackCooldown()
    {
        rangedAttackCooldownTimer = rangedAttackCooldown;
    }
    // 在 EliteController 中添加
    private void OnGUI()
    {
        if (IsRangedAttackOnCooldown)
        {
            GUI.Label(new Rect(10, 10, 200, 20), 
                $"Fireball Cooldown: {rangedAttackCooldownTimer.ToString("F1")}s");
        }
    }
  
    public bool FindVisibleTargets(out Vector2 toTarget)
    {
        // 获取视野范围内的所有玩家
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, viewRadius, targetMask);
        
        foreach (Collider2D targetCollider in targetsInViewRadius)
        {
    
            Vector2 dirToTarget = (PlayerManager.instance.player.transform.position- transform.position).normalized;
            
            // 检查是否在视野角度内
            if (Vector2.Angle(new Vector2(transform.localScale.x, 0), dirToTarget) < viewAngle / 2)
            {
                toTarget = dirToTarget;
                Debug.Log("找到目标");
                return true;
             
            }
        }
        toTarget = Vector2.zero;
        return false;
    }
    
    // 用于在编辑器中可视化视野范围
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
        
        Vector3 viewAngleA = DirFromAngle(-viewAngle / 2, false);
        Vector3 viewAngleB = DirFromAngle(viewAngle / 2, false);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.localScale.x*viewAngleA * viewRadius);
        Gizmos.DrawLine(transform.position, transform.position + transform.localScale.x*viewAngleB * viewRadius);
    }
    
    Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.z;
        }
        return new Vector3(Mathf.Cos(angleInDegrees * Mathf.Deg2Rad), Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0);
    }
    public  void FireFireball()
    {
        var fireball = 
            Instantiate(fireballPrefab, transform.position + offset, Quaternion.identity);
        Destroy(fireball, 3f);
    }
}