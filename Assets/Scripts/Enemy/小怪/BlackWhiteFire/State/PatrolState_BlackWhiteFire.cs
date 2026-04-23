using UnityEngine;

public class PatrolState_BlackWhiteFire : BlackWhiteFire_State
{
    
    [SerializeField] private int drawSegments = 30;   //绘制分段

    public override void OnEnter()
    {
        base.OnEnter();
        enemy.StartPatrol();

    }

    public override void OnExit()
    {
        base.OnExit();
        enemy.StopPatrol();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        PatrolLogic(enemy.detectionDistance, enemy.detectionAngle);  //索敌逻辑

    }


    protected virtual void PatrolLogic(float _detectionDistance, float _detectionAngle)//索敌逻辑
    {
        if (enemy == null)
            return;
        Vector3 center = enemy.transform.position;

        Vector3 forward = new Vector3(enemy.transform.localScale.x, 0, 0).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _detectionDistance, enemy.playerLayer);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - center).normalized;

            float angle = Vector3.Angle(forward, dirToTarget);


            if (angle <= _detectionAngle)
            {
                enemy.ChangeState(EnemyStateType.Chase);
            }
        }

        DrawCone(center, forward, enemy.detectionDistance, enemy.detectionAngle, drawSegments, Color.green);

    }


    protected virtual void DrawCone(Vector3 center, Vector3 forward, float radius, float angleRange, int segments, Color color)
    {
        // 计算扇形的一半角度，用于划分左右两侧
        float halfAngle = angleRange / 2f;

        //起始角度是 - halfAngle，将扇形对称地分成两边
        float startAngle = -halfAngle;

        //角度增量，决定扇形分段的精度
        float angleIncrement = angleRange / segments;

        //朝向
        Vector3 startDir = forward.normalized;

        for (int i = 0; i < segments; i++)
        {
            float angleA = startAngle + angleIncrement * i;
            float angleB = startAngle + angleIncrement * (i + 1);

            Vector3 dirA = Quaternion.Euler(0, 0, angleA) * startDir;
            Vector3 dirB = Quaternion.Euler(0, 0, angleB) * startDir;

            Vector3 pointA = center + dirA * radius;
            Vector3 pointB = center + dirB * radius;

            Debug.DrawLine(pointA, pointB, color);
        }


        Vector3 left = center + Quaternion.Euler(0, 0, -halfAngle) * startDir * radius;
        Vector3 right = center + Quaternion.Euler(0, 0, halfAngle) * startDir * radius;
        Debug.DrawLine(center, left, color);
        Debug.DrawLine(center, right, color);
    }
}
