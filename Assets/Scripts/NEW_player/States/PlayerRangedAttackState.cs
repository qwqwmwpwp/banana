using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedAttackState : PlayerState
{
    private float chargeStartTime; // 记录开始蓄力的时间
    private bool canRelease;       // 是否可以释放
    private bool isPaused;         // 动画是否在事件点暂停
    private TrackEntry trackEntry; // 当前动画实例

    public PlayerRangedAttackState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.zeroVelocity();

        canRelease = true;
        isPaused = false;
        chargeStartTime = Time.time;

        PlayAnimation();

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if(player.isCharging && Time.time - chargeStartTime >= player.chargeThreshold)
        {
            // 播放蓄力完成音效
        }

        if (isPaused && !player.isCharging && canRelease) 
        {
            // 判断蓄力时间
            if(Time.time - chargeStartTime >= player.chargeThreshold)
            {
                // 释放大火球
                player.ShootFireball(true);
            }
            else
            {
                // 释放小火球
                player.ShootFireball(false);

            }

            canRelease = false;
            trackEntry.TimeScale = 1f;
        }
    }

    private void PlayAnimation() // 播放动画
    {
        trackEntry = player.SetAnim(player.rangedAttackAnim, false);
        // 动画平滑过渡
        trackEntry.MixDuration = 0.1f;
        trackEntry.TrackTime = 0f;

        trackEntry.Complete -= OnAnimationComplete;
        trackEntry.Complete += OnAnimationComplete;

    }
    public void OnRangedAttackEvent()
    {
        if (trackEntry != null)
        {
            trackEntry.TimeScale = 0f; // 暂停动画
            isPaused = true;
        }
    }


    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        // 动画播放完后回到 Idle
        stateMachine.ChangeState(player.idleState);

    }

    

}
