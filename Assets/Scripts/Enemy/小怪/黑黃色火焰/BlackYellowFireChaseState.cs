using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DebugExtension;

public partial class BlackYellowFireController
{
    [Header("技能參數")]
    [SerializeField] public float attackSpeed;                      // 攻擊衝刺力度
    [SerializeField] public float attackRange;                      // 攻擊範圍 (到範圍內就發動衝刺)
    [SerializeField] public float attackCoolDownDuration;           // 攻擊冷卻
    [SerializeField] public float attackRecoveryDuration;           // 攻擊硬直
    [SerializeField] public float postAttackDelayDuration;          // 攻擊後搖 (動畫/移動持續時間)
    [SerializeField] public Vector2 attackBoxSize;                  // 攻擊碰撞盒大小
    [SerializeField] public Vector2 attackBoxOffset;                // 攻擊碰撞盒偏移
    [HideInInspector] public float attackRecoveryTimer;
    [HideInInspector] public float attackCoolDownTimer;

    [HideInInspector] public bool hasHitPlayer = false;             // 已攻擊判定 : 一次衝刺至多對玩家造成一次傷害
}


public class BlackYellowFireChaseState : BlackYellowFireStateBase
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
        controller.attackRecoveryTimer += Time.deltaTime;

        DrawAttackBox();
    }

    public override void OnFixedUpdate()
    {
        if (!transform.gameObject.activeSelf) return;

        // 检查玩家是否还存在
        if (playerTransform == null)
        {
            // 如果玩家不存在，停止移动并返回
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        MovePhysicsLogic();

        AttackPhysicsLogic();
    }

    // 移動邏輯
    protected virtual void MovePhysicsLogic()
    {
        if (controller.attackRecoveryTimer < controller.attackRecoveryDuration) return;

        // 再次检查玩家是否存在
        if (playerTransform == null) return;

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
        if (controller.attackRecoveryTimer < controller.attackRecoveryDuration) return;
        if (controller.attackCoolDownTimer < controller.attackCoolDownDuration) return;

        // 检查玩家是否存在
        if (playerTransform == null) return;

        float toPlayerDist = Vector3.Distance(transform.position, playerTransform.position);
        if (toPlayerDist < controller.attackRange)
        {
            // 呼叫 controller 啟動協程 (非 Mono 物件 不支持 Unity.Corountine, 需要利用 controller 調用協程)
            controller.StartCoroutine(Attack());

            controller.attackRecoveryTimer = 0;
            controller.attackCoolDownTimer = 0;
        }
    }

    // 攻擊
    protected virtual IEnumerator Attack()
    {
        float elapsed = 0f;
        bool hasHitPlayer = false;

        // 检查玩家是否存在
        if (playerTransform == null) yield break;

        Vector2 toPlayer = new Vector2(playerTransform.position.x - transform.position.x, 0).normalized;

        if (Mathf.Sign(toPlayer.x) != Mathf.Sign(transform.localScale.x))
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(toPlayer.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }

        while (elapsed < controller.postAttackDelayDuration)
        {
            // 每一帧都检查玩家是否存在
            if (playerTransform == null)
            {
                rb.velocity = Vector2.zero;
                yield break;
            }

            rb.velocity = toPlayer * controller.attackSpeed;
            elapsed += Time.fixedDeltaTime;
            yield return null;

            if (hasHitPlayer) continue;

            // 命中判定：每幀掃描
            Vector2 facingDir = new Vector2(transform.localScale.x, 0).normalized;
            Vector2 boxCenter = (Vector2)transform.position + Vector2.Scale(controller.attackBoxOffset, facingDir);
            Collider2D[] hits = Physics2D.OverlapBoxAll(boxCenter, controller.attackBoxSize, 0f, controller.playerLayer);
            foreach (var hit in hits)
            {
                Debug.Log("命中玩家: " + hit.name);
                hasHitPlayer = true;
                // TODO : 擊中邏輯
                if (hit.CompareTag("Player"))
                {
                    PlayerStats stats = hit.GetComponent<PlayerStats>();
                    if (stats != null)
                    {
                        stats.TakeDamage(controller.damage, AttackType.Melee);
                    }
                }
                
            }
        }

        rb.velocity = Vector2.zero;
    }

    protected virtual void DrawAttackBox()
    {
        Vector2 facingDir = new Vector2(transform.localScale.x, 0).normalized;
        Vector2 boxCenter = (Vector2)transform.position + Vector2.Scale(controller.attackBoxOffset, facingDir);

        DrawTools.DrawBounds(
                boxCenter,
                controller.attackBoxSize,
                controller.attackBoxOffset,
                new Color(255, 96, 0)
        );
    }

}
