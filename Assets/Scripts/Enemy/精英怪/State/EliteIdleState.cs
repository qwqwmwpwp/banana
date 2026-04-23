using System.Collections;
using System.Collections.Generic;
using Enemy.精英怪.State;
using UnityEngine;

public class EliteIdleState : EliteState
{
    private float Timer;
    [Header("等待時間")]
    public float IdleTime = 1f;
    public override void OnEnter()
    {
        // 重置计算器
        Timer = 0f;
        //TODO: 播放动画
    }
    public override void OnUpdate()
    {
        #region 检测玩家
        if (eliteController.FindVisibleTargets(out Vector2 tar))
        {
            eliteController.ChangeState(EliteStateType.EliteChase);
        }
        

        #endregion
        Timer+=Time.deltaTime;
        if(Timer > IdleTime) eliteController.ChangeState(EliteStateType.ElitePartol);
      
    }

    public override void OnExit()
    {
        
    }
}
