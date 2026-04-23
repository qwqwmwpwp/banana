using Enemy.Boss.Altar_Boos_2_;
using GameManager;
using UnityEngine;
/// <summary>
///无敌状态
/// </summary>
public class InvincibleState : StateBase
{
    AltarController controller;
        
         public override void Init(IStateMachineOwner owner)
         {
             controller = owner as AltarController;
         }

    public override void OnEnter()
    {
        //ToDo:播放动画
        controller.SetAnim(controller.InvincibleAnim, true);
        controller.stats.SetInvincible(true);
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnFixedUpdate()
    {
       
    }

    public override void OnLateUpdate()
    {
        
    }

    public override void OnExit()
    {
        controller.stats.SetInvincible(false);
    }
    
}
