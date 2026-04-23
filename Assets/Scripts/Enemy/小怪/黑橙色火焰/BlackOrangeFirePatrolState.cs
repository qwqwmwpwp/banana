using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DebugExtension;

public class BlackOrangeFirePatrolState : BlackOrangeFireStateBase
{
    private int patrolPointSize;
    private int patrolPointIndex = 0;

    public override void OnEnter()
    {
        transform = controller.transform;
        rb = controller.rb;
        
        patrolPointSize = controller.patrolPoints.Count;
        patrolPointIndex = Mathf.Clamp(patrolPointIndex, 0, patrolPointSize - 1);
    }

    public override void OnUpdate()
    {
        if (!transform.gameObject.activeSelf) return;

        controller.stayTimer += Time.deltaTime;
    }

    public override void OnFixedUpdate()
    {
        if (!transform.gameObject.activeSelf) return;
        
        MovePhysicsLogic();
        
        PatrolLogic();
    }

    protected virtual void MovePhysicsLogic()
    {       
        // 1. 檢查是否還在停留時間內
        if (controller.stayTimer < controller.stayDuration) return;

        // 2. 計算目標點資訊
        Vector3 targetPosition = controller.patrolPoints[patrolPointIndex].position;
        float distance = targetPosition.x - transform.position.x;
        float direction = Mathf.Sign(distance);

        // 3. 檢查是否抵達目標巡邏點
        if (Mathf.Abs(distance) < 1f)
        {
            patrolPointIndex = (patrolPointIndex + 1) % patrolPointSize;

            rb.velocity = Vector2.zero;

            controller.stayTimer = 0f;
            return; // 抵達了就直接停下，下一幀再看情況移動
        }

        // 4. 調整角色朝向
        transform.localScale = new Vector3(
            Mathf.Abs(transform.localScale.x) * direction, 
            transform.localScale.y, 
            transform.localScale.z);

        // 5. 設定移動速度
        if (!controller.groundChecker.isSloped)
        {
            rb.velocity = new Vector2(
                controller.moveSpeed * direction, 
                rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(
                -controller.moveSpeed * controller.groundChecker.slopeNormalPerp.x * direction,
                -controller.moveSpeed * controller.groundChecker.slopeNormalPerp.y * direction);
        }
    }

    protected virtual void PatrolLogic()
    {
        Vector3 center = transform.position;
        Vector3 forward = new Vector3(transform.localScale.x, 0, 0).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, controller.detectionRadius, controller.playerLayer);
        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - center).normalized;
            float angle = Vector3.Angle(forward, dirToTarget);
            if (angle <= controller.detectionAngle)
            {
                controller.ChangeState(EnemyCommon.EnemyCommonStateType.Chase);
                rb.velocity = Vector3.zero;
            }
        }

        DrawTools.DrawCone(center, forward, controller.detectionRadius, controller.detectionAngle, Color.green);
    }
}
