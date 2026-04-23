using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {

    }

    float timer;

    public override void Enter()
    {
        base.Enter();

        //进入无敌帧
        player.playerStats.isInvincibility = true;

        AudioManager.instance.PlaySFX(3);


        timer = player.dashDuration;

        PlayDashAnimation();

    }


    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        timer -= Time.deltaTime;
        if(timer<0)
            stateMachine.ChangeState(player.idleState);
        //进入无敌帧
        player.playerStats.isInvincibility = false;
    }
    private void PlayDashAnimation()
    {
        if (!player.DoingUltimateSkill)
        {
            // 如果不在大招状态，播放普通冲刺
            TrackEntry entry = player.SetAnim(player.dashAnim, false);
            // 动画平滑过渡
            entry.MixDuration = 0.1f;
            entry.TrackTime = 0f;
            
        }
        else
        {
            // 如果在大招状态，播放大招冲刺
            TrackEntry entry = player.SetAnim(player.ultimateDashAnim, false);
            // 动画平滑过渡
            entry.MixDuration = 0.1f;
            entry.TrackTime = 0f;

        }
    }


   
}
