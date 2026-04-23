using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DebugExtension;

public partial class BlackOrangeFireController
{
    [Header("碰撞盒參數")]
    [SerializeField] public Vector2 attackBoxSize;               // 碰撞盒大小
    [SerializeField] public Vector2 attackBoxOffset;             // 碰撞盒偏移
    [Tooltip("碰撞盒檢測間隔")]
    [SerializeField] public float colliderDuration;
    public float colliderTimer;                                  // 碰撞盒檢測間隔計時器

    [Header("技能參數")]
    [SerializeField] public float attackRange;                   // 攻擊範圍
    [SerializeField] public float attackCoolDownDuration;        // 攻擊冷卻
    public float attackCoolDownTimer;                            // 攻擊冷卻計時器
}

public class BlackOrangeFireChaseState : BlackOrangeFireStateBase
{
    public override void OnEnter()
    {
        transform = controller.transform;
        rb = controller.rb;
        controller.attackCoolDownTimer = 
            controller.attackCoolDownTimer == 0 ?
            controller.attackCoolDownDuration :
            controller.attackCoolDownTimer;
    }

    public override void OnUpdate()
    {
        if (!transform.gameObject.activeSelf) return;

        controller.attackCoolDownTimer += Time.deltaTime;
        controller.colliderTimer += Time.deltaTime;
    }

    public override void OnFixedUpdate()
    {
        if (!transform.gameObject.activeSelf) return;
        
        MovePhysicsLogic();

        AttackPhysicsLogic();

        ColliderAttackPhysicsLogic();
    }

    // 移動邏輯
    protected virtual void MovePhysicsLogic()
    {
        // 計算對玩家朝向
        float playerOffset = playerTransform.position.x - transform.position.x;
        float direction = Mathf.Sign(playerOffset);

        controller.transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * direction, 
            transform.localScale.y, 
            transform.localScale.z);

        // 朝向變換
        if (Mathf.Sign(controller.transform.localScale.x) != direction)
        {
            Vector3 scale = controller.transform.localScale;
            scale.x = Mathf.Abs(scale.y) * direction;
            controller.transform.localScale = scale;
        }

        
        // 支援斜坡
        if (!controller.groundChecker.isSloped)
        {
            rb.velocity = new Vector2(direction * controller.moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(
                -controller.moveSpeed * controller.groundChecker.slopeNormalPerp.x * direction,
                -controller.moveSpeed * controller.groundChecker.slopeNormalPerp.y * direction);
        }

        // 超出邊界則重置
        if (transform.position.x < Mathf.Min(controller.detectionPosMin.position.x, controller.detectionPosMax.position.x) ||
            transform.position.x > Mathf.Max(controller.detectionPosMin.position.x, controller.detectionPosMax.position.x) ||
            transform.position.y < Mathf.Min(controller.detectionPosMin.position.y, controller.detectionPosMax.position.y) ||
            transform.position.y > Mathf.Max(controller.detectionPosMin.position.y, controller.detectionPosMax.position.y))
        {
            controller.ResetSelf();
            return;
        }

    }

    // 攻擊邏輯
    protected virtual void AttackPhysicsLogic()
    {
        float toPlayerDist = Vector3.Distance(transform.position, playerTransform.position);
        
        if (toPlayerDist < controller.attackRange) rb.velocity = Vector2.zero;
        else return;

        if (controller.attackCoolDownTimer < controller.attackCoolDownDuration) return;
        
        RangeAttack();
        
        controller.attackCoolDownTimer = 0;
    }

    protected virtual void RangeAttack()
    {
        Vector2 center = transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, controller.attackRange, controller.playerLayer);

        foreach (var hit in hits)
        {
            Debug.Log("攻擊命中玩家: " + hit.name);

            // TODO : 在這裡觸發對玩家的傷害或其他效果
        }

        DrawTools.DrawCircle(center, controller.attackRange, Color.red, 0.5f);
    }

    // 碰撞盒邏輯
    protected virtual void ColliderAttackPhysicsLogic()
    {
        if (controller.colliderTimer < controller.colliderDuration) return;
        
        Vector2 facingDir = new Vector2(Mathf.Sign(transform.localScale.x), 0).normalized;
        Vector2 boxCenter = (Vector2)transform.position + Vector2.Scale(controller.attackBoxOffset, facingDir);
        Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, controller.attackBoxSize, 0f, controller.playerLayer);

        foreach (var hit in hits)
        {
            Debug.Log("碰撞盒命中玩家: " + hit.name);

            // TODO : 擊中邏輯
        }

        controller.colliderTimer = 0;

        DrawTools.DrawBounds(
            transform.position,
            controller.attackBoxSize,
            controller.attackBoxOffset * Mathf.Sign(transform.localScale.x),
            Color.red,
            0.5f);
    }
}
