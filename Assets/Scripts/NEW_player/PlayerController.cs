using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


// 改动：PlayerController
// PlayerAttackState
// PlayerIdleState
// PlayerGroundedState
public class PlayerController : Entity
{
    #region 动画部分

    [Header("动画信息")]
    public SkeletonAnimation animContorller;
    public Color originalColor;
    [Header("普通状态动画")]
    public AnimationReferenceAsset idleAnim; // 闲置
    public AnimationReferenceAsset moveAnim; // 跑步
    public AnimationReferenceAsset jumpAnim; // 起跳
    public AnimationReferenceAsset deadAnim; // 死亡
    public AnimationReferenceAsset dashAnim; // 冲刺
    public AnimationReferenceAsset hitAnim;  // 受击
    public AnimationReferenceAsset airAnim;  // 空中状态
    public AnimationReferenceAsset interactAnim;     // 交互
    public AnimationReferenceAsset airAttackAnim;    // 空中攻击
    public AnimationReferenceAsset chargeAttackAnim; // 普通蓄力近战
    public AnimationReferenceAsset rangedAttackAnim; // 地面远程攻击
    public AnimationReferenceAsset[] attackAnims;    // 地面近战连段
    [Header("大招状态动画")]
    public AnimationReferenceAsset ultimateAnim;      // 进入大招
    public AnimationReferenceAsset ultimateIdleAnim;  // 大招闲置
    public AnimationReferenceAsset ultimateMoveAnim;  // 大招跑步
    public AnimationReferenceAsset ultimateJumpAnim;  // 大招起跳
    public AnimationReferenceAsset ultimateDeadAnim;  // 大招死亡
    public AnimationReferenceAsset ultimateDashAnim;  // 大招冲刺
    public AnimationReferenceAsset ultimateHitAnim;   // 大招受击
    public AnimationReferenceAsset ultimateAirAnim;   // 大招空中状态
    public AnimationReferenceAsset ultimateAirAttackAnim; // 大招空中攻击
    public AnimationReferenceAsset ultimateInteractAnim;  // 大招交互
    public AnimationReferenceAsset[] ultimateAttackAnims; // 大招近战连段


    public TrackEntry SetAnim(AnimationReferenceAsset _anim, bool _loops)
    {

        // 用于切换动画
        TrackEntry entry = animContorller.AnimationState.SetAnimation(0, _anim, _loops);

        // TrackTime: 动画已经播放了多少时间,设置成 0f，把动画时间重置到开头，
        entry.TrackTime = 0f;

        //MixDuration 表示动画和上一个动画之间混合（过渡）的时间长度，设置成 0f，不要任何过渡，立刻切换。
        entry.MixDuration = 0f;

        return entry;
    }

    // 事件回调函数
    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        // 判断事件名字
        if (e.Data.Name == "attack")
        {
            // 事件来源的动画名
            string animName = trackEntry.Animation.Name;

            // 通知当前攻击状态执行攻击判定
            if (stateMachine.currentState is PlayerAttackState attackState)
            {
                attackState.AttackTrigger(); // 调用攻击判定函数
                Debug.Log($"Spine事件触发：{animName} 动画的 attack 事件");
            }
            if (stateMachine.currentState is PlayerChargeAttackState chargeAttack)
            {
                chargeAttack.AttackTrigger(); // 调用攻击判定函数
                Debug.Log($"Spine事件触发：{animName} 动画的 attack 事件");
            }
        }

        if (e.Data.Name == "rangedAttack")
        {
            // 只有当当前状态是远程攻击时才执行
            if (stateMachine.currentState is PlayerRangedAttackState rangedState)
            {
                rangedState.OnRangedAttackEvent();
            }
        }

        if (e.Data.Name == "dead")
        {

        }
    }
    #endregion

    #region  States 初始化与 Awake
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerHitState hitState { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    public PlayerAttackState attackState { get; private set; }
    public PlayerUltimateState ultimateState { get; private set; }
    public PlayerChargeAttackState chargeAttackState { get; private set; }
    public PlayerRangedAttackState rangedAttackState { get; private set; }
    protected override void Awake()
    {
        base.Awake();


        playerControls = new PlayerControls();

        stateMachine = new PlayerStateMachine();
        hitState = new PlayerHitState(this, stateMachine);
        idleState = new PlayerIdleState(this, stateMachine);
        moveState = new PlayerMoveState(this, stateMachine);
        jumpState = new PlayerJumpState(this, stateMachine);
        airState = new PlayerAirState(this, stateMachine);
        dashState = new PlayerDashState(this, stateMachine);
        deadState = new PlayerDeadState(this, stateMachine);
        attackState = new PlayerAttackState(this, stateMachine);           // 近战攻击连段
        ultimateState = new PlayerUltimateState(this, stateMachine);         // 进入大招
        chargeAttackState = new PlayerChargeAttackState(this, stateMachine); // 蓄力近战攻击
        rangedAttackState = new PlayerRangedAttackState(this, stateMachine); // 远程攻击
    }
    #endregion

    #region 各个参数
    [Header("prefabFX")]
    public GameObject chargeFX;     // 蓄力特效
    private Animator chargeFXanim;
    public GameObject AramorFX;     // 护盾特效

    [Header("攻击信息")]
    public Vector2[] attackMovemont; //攻击时略微移动增加打击感

    public float chargeThreshold = 1.5f;  // 蓄力阈值
    private float chargeStartTime;        // 记录开始蓄力的时间
    public bool isCharging;               // 是否正在蓄力
    private bool chargeFinishedPlayed;    // 蓄力完成音效是否已播放
    [Space]
    public GameObject fireballPrefab;
    public GameObject fireballPrefab_Big;
    public Transform firePoint;           // 发射点

    [Header("移动信息")]
    [HideInInspector] public float moveSpeed; // 移动速度
    [HideInInspector] public bool doubleJump; // 是否允许二段跳

    [Header("跳跃参数")]
    public float airControl = 3f;       // 空中水平控制的平滑度（越大越灵敏）
    public float jumpForce = 12f;       // 起跳时的爆发力
    public float gravityUp = 1.5f;      // 上升阶段的重力（数值越小，上升越慢）
    public float gravityDown = 3.5f;    // 下落阶段的重力（数值越大，下降越快）
    public float defaultGravity = 2.5f; // 默认重力
    [HideInInspector] public bool jumpHeld;    // 是否按住跳跃键


    [Header("冲刺信息")]
    public float dashSpeed;     // 冲刺速度
    public float dashDuration;  // 持续时间
    public float dashCooling;   // 冷却时间
    [HideInInspector] public float dashCoolTimer; // 计时器
    [HideInInspector] public float dashDir;       // 冲刺方向
    [HideInInspector] public bool dashedAir;      // 在空中冲刺过


    [Header("大招控制")]
    public bool ultimateIsFreeze;   //大招是否处于冻结
    public bool DoingUltimateSkill; //大招是否正在释放
    private float DoingUltimateSkillTimer;
    private float ultimateIsFreezeTimer;

    public PlayerControls playerControls;
    public PlayerStats playerStats;

    private Collider2D playerCollider;
    private bool isDropping = false;
    #endregion

    protected override void Start()
    {
        playerCollider = GetComponent<Collider2D>();

        if (animContorller == null)
        {
            Debug.LogError(" 未绑定 SkeletonAnimation 组件！");
            return;
        }

        // Spine旧版本没有 initialized 属性，用 this.IsValid() 来判断
        if (animContorller.skeleton == null)
        {
            animContorller.Initialize(true);
        }


        stateMachine.Initialize(idleState);
        playerStats = GetComponent<PlayerStats>();

        // 记录受击前的颜色
        originalColor = animContorller.skeleton.GetColor();

        // Spine事件监听（只需绑定一次）
        animContorller.AnimationState.Event += OnSpineEvent;

        AramorFX.SetActive(false);
        chargeFXanim = chargeFX.GetComponent<Animator>();

    }


    protected override void Update()
    {
        // State脚本没有挂载到实体上运行，通过这里的Update调用
        stateMachine.currentState.Update();

       

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (IsStandingOnOneWayPlatform())
                StartCoroutine(DropThroughPlatform());
        }

        xInput = playerControls.GamePlay.Move.ReadValue<Vector2>().x;
        yInput = playerControls.GamePlay.Move.ReadValue<Vector2>().y;

        // 翻转与翻转屏蔽
        if (stateMachine.currentState != dashState &&
            stateMachine.currentState != attackState &&
            stateMachine.currentState != chargeAttackState &&
            stateMachine.currentState != ultimateState)
        {
            if (xInput != facingDer && xInput != 0)
            {
                Flip(cameraFollow);
            }
        }

        // 进入地面时，刷新二段跳与空中冲刺
        dashCoolTimer -= Time.deltaTime;
        if (isGrounded)
        {
            doubleJump = true;
            dashedAir = false;
        }

        // 大招的持续与冷却控制
        DoingUltimateSkillTimer -= Time.deltaTime;
        ultimateIsFreezeTimer -= Time.deltaTime;
        if (DoingUltimateSkill)
        {
            if (DoingUltimateSkillTimer < 0)
            {
                DoingUltimateSkill = false;

                ultimateIsFreezeTimer = playerStats.ultimateFreezeDuration.GetValue();
                ultimateIsFreeze = true;
                UIManager.instance.GameUI.StartFreezeUltimate();
                UIManager.instance.GameUI.SetUltimateFX(false);

                AudioManager.instance.PlaySFX(23);

                stateMachine.ChangeState(idleState);
            }
        }
        if (ultimateIsFreeze)
        {
            if (ultimateIsFreezeTimer < 0)
            {
                ultimateIsFreeze = false;
                UIManager.instance.GameUI.CloseFreezeUltimate();

            }

        }

        chargeFXanim.SetBool("Charge", isCharging);

        // 蓄力音效检测逻辑
        if (isCharging)
        {
            float chargeTime = Time.time - chargeStartTime;

            // 达到蓄力阈值时只播放一次完成音效
            if (chargeTime >= chargeThreshold && !chargeFinishedPlayed)
            {
                AudioManager.instance.PlaySFX(10);
                chargeFinishedPlayed = true;
            }
        }
    }



    // 发射火球
    public void ShootFireball(bool isCharg)
    {
        if (!isCharg)
        {
            GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
            PlayerRangedAttackFire fb = fireball.GetComponent<PlayerRangedAttackFire>();
            fb.SetDirection(facingRight, isCharg);
            AudioManager.instance.PlaySFX(12);

        }
        else
        {
            GameObject fireball = Instantiate(fireballPrefab_Big, firePoint.position, Quaternion.identity);
            PlayerRangedAttackFire fb = fireball.GetComponent<PlayerRangedAttackFire>();
            fb.SetDirection(facingRight, isCharg);
            AudioManager.instance.PlaySFX(13);

        }

    }



    // 检测玩家是否站在单向平台上
    private bool IsStandingOnOneWayPlatform()
    {
        Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, whatIsGround);
        if (col != null && col.GetComponent<PlatformEffector2D>() != null)
        {
            return true; // 确认脚下是带 Effector 的平台
        }
        return false;
    }

    private IEnumerator DropThroughPlatform()
    {
        if (isDropping) yield break;
        isDropping = true;

        Collider2D col = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, whatIsGround);
        if (col != null && col.TryGetComponent(out PlatformEffector2D effector))
        {
            // 临时忽略碰撞
            Physics2D.IgnoreCollision(playerCollider, col, true);
            yield return new WaitForSeconds(0.5f);
            Physics2D.IgnoreCollision(playerCollider, col, false);
        }

        isDropping = false;
    }

    // 在 Scene 视图中显示检测范围
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckDistance);
        }
    }

    #region 按键映射

    public bool inputAttack = false; // 是否按下攻击键
    public bool inputJump = false;   // 是否按下跳跃键

    private void OnEnable()
    {
        // 启用 GamePlay 动作映射
        playerControls.GamePlay.Enable();

        playerControls.GamePlay.Jump.started += OnJumpStarted;
        playerControls.GamePlay.Jump.canceled += OnJumpCanceled;

        playerControls.GamePlay.Ultimate.performed += Ultimate;

        playerControls.GamePlay.Dash.performed += Dash;

        playerControls.GamePlay.Attack.started += OnAttackStarted;
        playerControls.GamePlay.Attack.canceled += OnAttackCanceled;
        playerControls.GamePlay.Attack.performed += ctx => inputAttack = true;
        playerControls.GamePlay.Attack.canceled += ctx => inputAttack = false;

        playerControls.GamePlay.RangedAttack.started += OnRangedAttackStarted;
        playerControls.GamePlay.RangedAttack.canceled += OnRangedAttackCanceled;

    }



    private void OnDisable()
    {
        playerControls.GamePlay.Disable(); // 关闭，防止内存泄露
    }

    // 跳跃
    private void OnJumpStarted(InputAction.CallbackContext context)
    {
        // 正在按住跳跃
        jumpHeld = true;

        if (isGrounded)
        {
            stateMachine.ChangeState(jumpState);
        }
        else if (!isGrounded && doubleJump)
        {
            stateMachine.ChangeState(jumpState);
            doubleJump = false;
        }
    }

    private void OnJumpCanceled(InputAction.CallbackContext context)
    {
        jumpHeld = false;
    }

    // 大招
    private void Ultimate(InputAction.CallbackContext context)
    {
        if (playerStats.currrentUltimateEnergy == 100 && !ultimateIsFreeze && !DoingUltimateSkill && isGrounded)
        {
            // 大招启动
            AudioManager.instance.PlaySFX(21);

            // 如果当前大招能量满了，消耗能量，进入大招动画
            playerStats.currrentUltimateEnergy = 0;

            // 标识大招状态
            DoingUltimateSkillTimer = playerStats.UltimateDuration;
            DoingUltimateSkill = true;

            // 更新UI
            UIManager.instance.GameUI.SetUltimateFX(true);

            // 进入状态
            stateMachine.ChangeState(ultimateState);

        }
        else if (!DoingUltimateSkill)
        {
            //抖动效果提示
            UIManager.instance.GameUI.EnergyUIShake();
        }
    }

    //冲刺
    private void Dash(InputAction.CallbackContext context)
    {
        if (dashCoolTimer > 0)
            return;

        if (xInput != 0)
            dashDir = xInput;
        else
            dashDir = facingDer;

        if (!isGrounded && !dashedAir)
        {
            // 如果不在地面且没有空中冲刺过
            stateMachine.ChangeState(dashState);

        }
        if (isGrounded)
        {
            if (stateMachine.currentState != dashState)
                stateMachine.ChangeState(dashState);
        }
        dashCoolTimer = dashCooling;
    }


    // 按下攻击键：开始蓄力
    private void OnAttackStarted(InputAction.CallbackContext context)
    {


        // 蓄力循环音效
        AudioManager.instance.PlayLoopSFX(9);

        chargeStartTime = Time.time;
        isCharging = true;

    }
    // 松开攻击键：根据时长判断普通攻击/蓄力攻击
    private void OnAttackCanceled(InputAction.CallbackContext context)
    {
        // 停止蓄力循环音效
        AudioManager.instance.StopLoopSFX(9);

        isCharging = false;
        chargeFinishedPlayed = false;

        if (Time.time - chargeStartTime >= chargeThreshold)
        {

            // 如果蓄力时间大于蓄力阈值，进入蓄力攻击
            stateMachine.ChangeState(chargeAttackState);
        }
        else
        {
            // 否则进入普通近战
            if (stateMachine.currentState != attackState)
                stateMachine.ChangeState(attackState);
        }


    }


    // 开始远程蓄力
    private void OnRangedAttackStarted(InputAction.CallbackContext context)
    {
        // 大招期间禁止远程攻击
        if (DoingUltimateSkill)
            return;

        // 重新记录蓄力开始时间
        chargeStartTime = Time.time;
        isCharging = true;
        chargeFinishedPlayed = false;

        // 播放循环蓄力音效
        AudioManager.instance.PlayLoopSFX(9);

        if (stateMachine.currentState != rangedAttackState)
            stateMachine.ChangeState(rangedAttackState);
    }
    // 松开远程蓄力
    private void OnRangedAttackCanceled(InputAction.CallbackContext context)
    {
        // 停止蓄力循环音效
        AudioManager.instance.StopLoopSFX(9);
        isCharging = false;
        chargeFinishedPlayed = false;
    }
    #endregion
}
