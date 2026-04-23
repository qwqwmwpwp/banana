using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();


    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (rb.velocity.y < 0)
        {
            // 下落阶段：增加重力，让落地更重、手感更实
            rb.gravityScale = player.gravityDown;

            // 当y轴速度向下时，播放对应下落动画
            if (player.DoingUltimateSkill)
                player.SetAnim(player.ultimateAirAnim, true);
            else
                player.SetAnim(player.airAnim, true);
        }

        // 一旦接触地面，切换回Idle或Move状态
        if (player.isGrounded)
        {
            rb.gravityScale = player.defaultGravity; // 恢复重力

            AudioManager.instance.PlaySFX(1);


            if (Mathf.Abs(player.xInput) > 0.1f)
                stateMachine.ChangeState(player.moveState); // 落地后移动
            else
                stateMachine.ChangeState(player.idleState); // 落地静止
        }

        if (player.xInput != 0)
        {
            // 空中控制角色的水平移动，比地面稍慢
            player.SetVelocity(player.moveSpeed * 0.8f * player.xInput, rb.velocity.y);
        }

        
    }
}
