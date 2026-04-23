using Spine;
using UnityEngine;

public class PlayerDeadState : PlayerState
{
    public PlayerDeadState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //进入无敌帧
        player.playerStats.isInvincibility = true;

        PlayAnimation();
        AudioManager.instance.PlaySFX(0);

        // 禁止输入
        player.playerControls.GamePlay.Disable();
    }
    
    public override void Exit()
    {
        base.Exit();
        //进入无敌帧
        player.playerStats.isInvincibility = false;
    }

    public override void Update()
    {
        base.Update();
    }



    private void PlayAnimation()
    {
        TrackEntry entry;

        if (!player.DoingUltimateSkill)
        {
            entry = player.SetAnim(player.deadAnim, false);
        }
        else
        {
            entry = player.SetAnim(player.ultimateDeadAnim, false);
        }

        // 平滑过渡到当前动画
        entry.MixDuration = 0.1f;
        entry.TrackTime = 0f;

        // 绑定新的回调（TrackEntry 是新的实例）
        entry.Complete -= OnAnimationComplete;
        entry.Complete += OnAnimationComplete;

    }

    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        // 黑屏淡入淡出
        UIManager.instance.FadeInOut(() => DeathSequence());

    }

    private void DeathSequence()
    {
        // 血量护盾回满
        player.stats.RestoreHealth(1f);
        player.stats.FullyRestoreArmor();

        // 回到复活点
        player.rb.position = PlayerManager.instance.respawnPoint.position;
        player.rb.velocity = Vector2.zero;

        // 标记复活
        player.stats.isDie = false;

        // 恢复输入
        player.playerControls.GamePlay.Enable();

        // 切回 Idle 状态（或其他正常状态）
        player.stateMachine.ChangeState(player.idleState);


    }
}
