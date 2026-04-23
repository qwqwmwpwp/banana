using Enemy.Boss.Altar_Boos_2_.Status;
using Spine;
using UnityEngine;
using AnimationState = Spine.AnimationState;

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(PlayerController _player, PlayerStateMachine _stateMachine) : base(_player, _stateMachine) { }

    private int comboCounter;         // 当前连击段数
    private float lastTimeAttacked;   // 上次攻击时间
    private bool nextAttack;          // 动画中是否有预输入
    private float nextWindow = 0.3f;  // 预输入时间窗口（动画中按下触发下一段）

    private bool waitingForNext;      // 后摇等待标记（动画结束后0.5s可继续连击）
    private float afterWindow = 0.5f; // 后摇窗口时间
    private float waitStartTime;      // 后摇开始时间

    public override void Enter()
    {

        base.Enter();

        if (player.isGrounded)
            player.zeroVelocity();

        nextAttack = false;
        waitingForNext = false;

        // 播放第一段攻击动画
        PlayAnimation();
    }

    public override void Exit()
    {
        base.Exit();
        comboCounter = 0;
        nextAttack = false;
        waitingForNext = false;
    }

    public override void Update()
    {
        base.Update();

        // 玩家按下攻击键
        if (player.inputAttack)
        {
            AnimationState state = player.animContorller.AnimationState;
            TrackEntry currentEntry = state.GetCurrent(0);
            if (currentEntry != null)
            {
                float remainingTime = currentEntry.AnimationEnd - currentEntry.TrackTime;
                if (player.inputAttack && remainingTime <= nextWindow)
                {
                    nextAttack = true; // 动画进入后半段按下触发下一段
                }
            }
            // 后摇窗口内按下攻击键进入下一段攻击
            else if (waitingForNext && Time.time - waitStartTime <= afterWindow)
            {
                waitingForNext = false;
                comboCounter++;
                PlayAnimation();
            }
        }

        // 玩家在后摇期间有移动输入，立即切换到移动状态
        if (waitingForNext && Mathf.Abs(player.xInput) > 0)
        {
            waitingForNext = false;
            comboCounter = 0;
            stateMachine.ChangeState(player.moveState);
            return;
        }

        // 超过后摇时间
        if (waitingForNext && Time.time - waitStartTime > afterWindow)
        {
            waitingForNext = false;
            comboCounter = 0;

            if (player.xInput == 0)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.moveState);

        }
    }

    // 播放当前连击段动画
    private void PlayAnimation()
    {
        
        TrackEntry entry;

        if (!player.DoingUltimateSkill)
        {
            entry = player.SetAnim(player.attackAnims[comboCounter], false);

        }
        else
        {
            entry = player.SetAnim(player.ultimateAttackAnims[comboCounter], false);
           

        }

        if (player.isGrounded)
        {
            // 地面攻击时可以加入位移打击感，不改变空中攻击的水平速度
            player.SetVelocity(player.attackMovemont[comboCounter].x * player.facingDer,
                               player.attackMovemont[comboCounter].y);
        }


        // 平滑过渡到当前动画
        entry.MixDuration = 0.1f;
        entry.TrackTime = 0f;

        // 绑定动画完成回调，先移除避免重复绑定
        entry.Complete -= OnAnimationComplete;
        entry.Complete += OnAnimationComplete;

        // 动态设置预输入窗口
        nextWindow = Mathf.Min(0.3f, entry.AnimationEnd * 0.5f); // 预输入窗口 = 动画前半段或最大0.3秒

        PlaySFX();
    }

    // 动画播放完成回调
    private void OnAnimationComplete(TrackEntry trackEntry)
    {
        player.zeroVelocity();
        lastTimeAttacked = Time.time;

        bool hasNextCombo; // 是否还有下一段攻击

        if (!player.DoingUltimateSkill)
            hasNextCombo = comboCounter < player.attackAnims.Length - 1;
        else
            hasNextCombo = comboCounter < player.ultimateAttackAnims.Length - 1;

        // 动画播放完，有预输入：立即进入下一段
        if (nextAttack && hasNextCombo)
        {
            nextAttack = false;
            comboCounter++;
            PlayAnimation();
        }
        // 动画播放完，没有预输入，但还有后续连击 ： 进入后摇窗口
        else if (hasNextCombo)
        {
            waitingForNext = true;
            waitStartTime = Time.time;

            // 为避免卡在最后一帧，播放 Idle 动画过渡
            if (!(player.DoingUltimateSkill && comboCounter == 1)) //避免普通攻击第二段太快
                player.SetAnim(player.idleAnim, true).MixDuration = 0.05f;
        }
        // 没有后续连击：回 Idle
        else
        {
            comboCounter = 0;

            if (player.xInput == 0)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.moveState);
        }
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
                PlayHitSFX();

                float attackMultiplier;

                if (!player.DoingUltimateSkill)
                {
                    if (comboCounter != 0)
                        attackMultiplier = 1.2f;
                    else
                        attackMultiplier = 1f;
                }
                else
                {
                     attackMultiplier = 3f;
                }
                player.playerStats.DoDamage(targetStats, AttackType.Melee, attackMultiplier);
                Debug.Log(targetStats.name);
            }
            else if (hit.CompareTag("祭坛"))
            {
               
                AltarStatus altarStats = hit.GetComponentInParent<AltarStatus>();
                altarStats.CheckBuff();
            }
        }
    }

    private void PlaySFX()
    {
        if((comboCounter == 0 || comboCounter == 2) && !player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(5);
        else if(comboCounter == 1 && !player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(6);
        else if (comboCounter == 0 && player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(7);
        else if (comboCounter == 1 && player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(8);
    }
    private void PlayHitSFX()
    {
        if((comboCounter == 0 || comboCounter == 2) && !player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(5);
        else if(comboCounter == 1 && !player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(6);
        else if (comboCounter == 0 && player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(20);
        else if (comboCounter == 1 && player.DoingUltimateSkill)
            AudioManager.instance.PlaySFX(21);
    }

}
