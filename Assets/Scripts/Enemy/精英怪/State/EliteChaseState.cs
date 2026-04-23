

using System.Collections;
using System.Collections.Generic;
using Enemy.精英怪.State;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.UI;


public class EliteChaseState : EliteState
{
    [Header("追击速度")]
    public float speed = 10f;
    [Header("近战距离")] 
    public float closeAttackDistance = 6f;
    public float Timer;
    public override void OnEnter()
    {
        Timer = 0f;
        //TODO: 播放动画
    }
    public override void OnUpdate()
    {
        Timer += Time.deltaTime;
        //发现玩家
        if (eliteController.FindVisibleTargets(out Vector2 dir)|| Timer <= 1f )
        {
            eliteController.rb.velocity = dir * speed;
            // 如果远程攻击不在冷却中且距离足够，切换到远程攻击

                float distance = Mathf.Abs(PlayerManager.instance.player.transform.position.x- eliteController.transform.position.x);  
                //根据距离选择攻击方式
                if ( distance <= closeAttackDistance)
                {
                    Debug.Log("选择近战攻击");
                    eliteController.ChangeState(EliteStateType.EliteCloseAttack);
                    return;
                }
                    else
                {
                    if(!eliteController.IsRangedAttackOnCooldown)
                        eliteController.ChangeState(EliteStateType.EliteRemoteAttack);
                    return;
                }
                
            
        }
        else
        {
            eliteController.ChangeState(EliteStateType.ElitePartol);
        }
            
        
    }

    public override void OnExit()
    {
        
    }
    
}