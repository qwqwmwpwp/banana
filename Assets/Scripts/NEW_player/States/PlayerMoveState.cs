using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        AudioManager.instance.PlayLoopSFX(4);

        // 恢复移动：
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // 播放动画
        if (player.DoingUltimateSkill)
            player.SetAnim(player.ultimateMoveAnim, true);
        else
            player.SetAnim(player.moveAnim, true);

    }

    public override void Exit()
    {
        base.Exit();
        AudioManager.instance.StopLoopSFX(4);
    }

    public override void Update()
    {
        base.Update();

        // 水平移动
        rb.velocity = new Vector2(player.xInput * player.moveSpeed, rb.velocity.y);

        // 若没有输入 → 待机
        if (Mathf.Abs(player.xInput) < 0.01f)
        {
            stateMachine.ChangeState(player.idleState);
            return;
        }

        // 若离开地面 → 空中状态
        if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.airState);
        }
    }
}
