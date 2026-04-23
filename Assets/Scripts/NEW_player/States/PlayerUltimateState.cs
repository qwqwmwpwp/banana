using Spine;
using Spine.Unity;
using UnityEngine;

public class PlayerUltimateState : PlayerGroundedState
{
    
    public PlayerUltimateState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }
    public override void Enter()
    {
        base.Enter();

        player.zeroVelocity();

        TrackEntry entry = player.SetAnim(player.ultimateAnim, false);
        entry.Complete += OnAnimationComplete;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();


    }
    void OnAnimationComplete(TrackEntry trackEntry) // 动画播放完成一遍时调用
    {
        stateMachine.ChangeState(player.idleState);
    }
}
