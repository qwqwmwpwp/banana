using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerState
{
    public PlayerHitState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rb.velocity = new Vector2(0, rb.velocity.y);

        // 播放动画
        PlayAnimation();
        //进入无敌帧
        player.playerStats.isInvincibility = true;


        // 受击闪烁
        PlayFlash();
    }

    public override void Exit()
    {
        base.Exit();
        player.playerStats.isInvincibility = false;
    }

    public override void Update()
    {
        base.Update();
    }

    private void PlayAnimation() // 播放动画
    {
        if (!player.DoingUltimateSkill)
        {
            TrackEntry entry = player.SetAnim(player.hitAnim, false);
            // 动画平滑过渡
            entry.MixDuration = 0.1f;
            entry.TrackTime = 0f;

            entry.Complete -= OnAnimationComplete;
            entry.Complete += OnAnimationComplete;
        }
        else
        {
            TrackEntry entry = player.SetAnim(player.ultimateHitAnim, false);
            // 动画平滑过渡
            entry.MixDuration = 0.1f;
            entry.TrackTime = 0f;

            entry.Complete -= OnAnimationComplete;
            entry.Complete += OnAnimationComplete;
        }


    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        stateMachine.ChangeState(player.idleState);
    }

    [SerializeField] private Color flashColor = Color.red; // 闪烁颜色
    [SerializeField] private int flashCount = 3;           // 闪烁次数
    [SerializeField] private float flashDuration = 0.1f;   // 每次闪烁时长


    private void PlayFlash()
    {
        // 先停止之前的闪烁，避免叠加
        DOTween.Kill(player.animContorller.skeleton);
        /// <summary>
        /// SetLoops（重复次数, SetLoops）：
        /// 
        /// 【LoopType.Restart】
        /// 每次都从 起点 开始重新播放。
        /// A → B, A → B
        /// 【LoopType.Yoyo】
        /// 正向一次，反向一次
        /// A → B, B → A
        /// 【LoopType.Incremental】
        /// 每次循环，都会在目标值上累加。
        /// A → B, B → C, C → D...
        /// </summary>
        player.animContorller.skeleton
            .DOColor(flashColor, flashDuration)      // 变成闪烁颜色(一次动画)
            .SetLoops(flashCount * 2, LoopType.Yoyo) // 对 DOColor 返回的动画做循环操作
            .OnComplete(() =>
            {
                // 当 Tween 正常完成所有循环并到达终点 时触发一次回调，闪烁结束恢复原始颜色
                player.animContorller.skeleton.SetColor(player.originalColor);
            })
            .OnKill(() =>
            {
                // 如果 tween 被提前 Kill 时触发一次回调，确保恢复颜色
                if (player != null && player.animContorller != null)
                    player.animContorller.skeleton.SetColor(player.originalColor);
            });

    }
}
