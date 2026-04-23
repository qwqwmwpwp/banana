using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerChargeAttackState : PlayerState
{
    public PlayerChargeAttackState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        PlayAnimation();

        AudioManager.instance.PlaySFX(11);

    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();
    }

    private void PlayAnimation() // 播放动画
    {
        TrackEntry entry = player.SetAnim(player.chargeAttackAnim, false);
        entry.MixDuration = 0.1f;
        entry.TrackTime = 0f;

        // 播放动画时添加一个冲刺效果
        if (player.isGrounded)
        {
            float chargeDashDistance = 3f;     // 位移距离
            float chargeDashDuration = 0.2f;   // 位移耗时
            float dashSpeed = chargeDashDistance / chargeDashDuration;

            // 设置一个短暂的水平速度
            player.SetVelocity(player.facingDer * dashSpeed, player.rb.velocity.y);

            // 过一小段时间恢复正常速度
            player.StartCoroutine(StopDashAfterTime(chargeDashDuration));
        }

        entry.Complete -= OnAnimationComplete;
        entry.Complete += OnAnimationComplete;
    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        stateMachine.ChangeState(player.idleState);
    }

    private IEnumerator StopDashAfterTime(float t)
    {
        yield return new WaitForSeconds(t);
        player.zeroVelocity();
    }


    // 攻击判定
    public void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);
        
        foreach (var hit in colliders)
        {
            EnemyStats targetStats = hit.GetComponent<EnemyStats>();
            if (targetStats == null) targetStats = hit.GetComponentInParent<EnemyStats>();
            if (targetStats != null)
            {
                player.playerStats.DoDamage(targetStats, AttackType.Melee, 2.5f);
                Debug.Log(player.playerStats.damage.GetValue());
            }
        }
    }
}
