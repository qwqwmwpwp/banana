using UnityEngine;

public class IdleState_BlackRedFire_Ranged : BlackRedFire_Ranged_State
{
    
    [SerializeField] private int drawSegments = 30;   

    public override void OnEnter()
    {
        base.OnEnter();

   
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnFixedUpdate();

        if(enemy!=null)
            PatrolLogic(enemy.detectionDistance, enemy.detectionAngle); 
    }


    protected virtual void PatrolLogic(float _detectionDistance, float _detectionAngle)
    {
        Vector3 center = enemy.transform.position;

        Vector3 forward = new Vector3(enemy.transform.localScale.x, 0, 0).normalized;

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, _detectionDistance, enemy.playerLayer);

        foreach (var hit in hits)
        {
            Vector3 dirToTarget = (hit.transform.position - center).normalized;

            float angle = Vector3.Angle(forward, dirToTarget);


            if (angle <= _detectionAngle && enemy.fireballCooldownTimer < 0)
            {
                enemy.ChangeState(EnemyStateType.Charging);
            }
        }

        DrawCone(center, forward, enemy.detectionDistance, enemy.detectionAngle, drawSegments, Color.green);

    }


    protected virtual void DrawCone(Vector3 center, Vector3 forward, float radius, float angleRange, int segments, Color color)
    {
        float halfAngle = angleRange / 2f;

        float startAngle = -halfAngle;

        float angleIncrement = angleRange / segments;

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
