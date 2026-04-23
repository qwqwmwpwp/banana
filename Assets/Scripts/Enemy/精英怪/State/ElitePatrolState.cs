using System.Collections;
using System.Collections.Generic;
using Enemy.精英怪.State;
using UnityEngine;


public class ElitePatrolState : EliteState
{
    private int patrolpointSize;   // 巡逻点总数
    int pointIndex = 0;            // 巡逻点索引
 
    [Header("速度")]
    public float speed = 5f;
    public override void OnEnter()
    {
        patrolpointSize = eliteController.patrolPoints.Length;
        // 确保索引在有效范围内
        pointIndex = Mathf.Clamp(pointIndex, 0, patrolpointSize - 1);
        // TODO: 播放动画
    }

    public override void OnUpdate()
    {

    }
       
    public override void OnFixedUpdate()
    {
        Transform nextTarget = eliteController.patrolPoints[pointIndex];
        float distance = nextTarget.position.x - eliteController.transform.position.x;

        #region 检测玩家
      
        if (eliteController.FindVisibleTargets(out Vector2 tar))
        {
            Debug.Log("巡逻时检测到了");
            eliteController.ChangeState(EliteStateType.EliteChase);
            return;
        }
        

        #endregion
        // 计算移动方向
        Vector2 dir = new Vector2(distance, 0).normalized;
        
        eliteController.rb.velocity = new Vector2(dir.x * speed, eliteController.rb.velocity.y);
        eliteController.transform.localScale = new Vector3(dir.x*eliteController.transform.localScale.y, eliteController.transform.localScale.y, eliteController.transform.localScale.z);
        // 使用绝对值判断是否到达目标点
        if (Mathf.Abs(distance) < 0.1f)
        {
            eliteController.rb.velocity = Vector2.zero;
            pointIndex = (pointIndex + 1) % patrolpointSize; // 循环索引
            eliteController.ChangeState(EliteStateType.EliteIdle);
        }
    }

    public override void OnLateUpdate()
    {
        
    }

    public override void OnExit()
    {
       
    }
}
