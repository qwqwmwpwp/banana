using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    // 保存默认重力，以便退出状态时恢复
    private float defaultGravity;
    // 是否已经松开跳跃键（用于控制短按跳）
    private bool hasReleasedJump;

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlaySFX(2);


        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 重置状态
        hasReleasedJump = false;
        defaultGravity = rb.gravityScale;

        // 播放跳跃动画（区分是否为大招状态）
        if (player.DoingUltimateSkill)
            player.SetAnim(player.ultimateJumpAnim, true);
        else
            player.SetAnim(player.jumpAnim, true);

        // 初始爆发：瞬间给予向上的速度
        float xSpeed = player.xInput * player.moveSpeed;
        player.SetVelocity(xSpeed , player.jumpForce);
    }

    public override void Exit()
    {
        base.Exit();

        // 退出时重置重力
        rb.gravityScale = defaultGravity;
    }

    public override void Update()
    {
        base.Update();

        // 如果松开跳跃键但尚未标记释放，则削减当前上升速度
        if (!player.jumpHeld && !hasReleasedJump)
        {
            hasReleasedJump = true;
            // 截断上升
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f); 
        }

        // 根据角色上升/下降调整重力强度
        if (rb.velocity.y > 0 && player.jumpHeld)
        {
            // 上升阶段：减弱重力，使上升更柔和（长按跳更高）
            rb.gravityScale = player.gravityUp;
        }
        else if (rb.velocity.y < 0)
        {
            //进入空中状态
            stateMachine.ChangeState(player.airState);
        }

        // 空中时可调整方向，但带惯性（平滑过渡）
        if (player.xInput != 0)
        {
            float targetSpeed = player.moveSpeed * player.xInput;

            // Mathf.Lerp 平滑插值：模拟惯性移动效果
            float smoothX = Mathf.Lerp(rb.velocity.x, targetSpeed, Time.deltaTime * player.airControl);

            player.SetVelocity(smoothX, rb.velocity.y);

        }

    }

}
