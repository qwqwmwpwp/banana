using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 在地面进入时速度归零，防止滑行
        if (player.isGrounded)
            player.zeroVelocity();



        // 播放动画
        if (player.DoingUltimateSkill)
            player.SetAnim(player.ultimateIdleAnim, true);
        else
            player.SetAnim(player.idleAnim, true);

    }

    public override void Exit()
    {
        base.Exit();

        // 离开idle时恢复正常约束
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }




    public override void Update()
    {
        base.Update();

        if (player.CheckSlope())
        {
            // 冻结水平移动（防止沿坡滑）
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }
        if (player.xInput != 0 && player.isGrounded && !DialogueManager.instance.isDialogue)
        {
            // 如果在地面且不在对话
            stateMachine.ChangeState(player.moveState);
        }

        if (!player.isGrounded)
        {
            stateMachine.ChangeState(player.airState);
        }

    }


}
