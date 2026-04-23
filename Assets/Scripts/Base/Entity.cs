using PhysicsExtension;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Entity : MonoBehaviour
{
    #region === 组件引用 ===
    public virtual Rigidbody2D rb { get; private set; }        // 刚体组件（控制物理运动）
    public virtual EntityFX fx { get; private set; }           // 特效控制（闪烁、受击效果等）
    public virtual SpriteRenderer sr { get; private set; }     // 精灵组件
    public virtual CharacterStats stats { get; private set; }  // 属性数据（血量、攻击力等）
    #endregion

    public GameObject cameraFollow;  // 玩家或敌人的朝向基准物体（翻转时一并旋转）

    #region === 击退信息 ===
    [Header("击退信息")]
    [SerializeField] protected Vector2 knockbackPower;  // 击退力度 
    [SerializeField] protected float knockbackDuration; // 击退持续时间
    protected bool isKnockback;                         // 当前是否处于击退状态（锁定移动）
    public int knockbackDer;                            // 击退方向 
    #endregion

    public System.Action onFlipped; // 翻转事件回调（例如通知武器或UI同步方向）

    #region === 地面检测与输入信息 ===
    [Header("检测信息")]
    [SerializeField] protected Transform groundCheck;            // 地面检测点
    [SerializeField] protected float groundCheckDistance = 0.2f; // 检测半径
    [SerializeField] protected LayerMask whatIsGround;           // 地面层
    [SerializeField] protected float slopeCheckDistance = 0.5f;  // 斜坡检测距离
    [SerializeField] protected float coyoteTime = 0.08f;         // “土狼时间”，防止落地抖动

    public Transform attackCheck;    // 攻击检测圆心
    public float attackCheckRadius;  // 攻击检测范围

    public float xInput;             // 水平输入
    public float yInput;             // 垂直输入
    public int facingDer { get; private set; } = 1;  // 朝向方向（1=右，-1=左）
    protected bool facingRight = true;               // 朝向标记
    #endregion

    #region === 状态变量 ===
    public bool isGrounded { get; private set; }     // 当前是否接触地面
    private bool wasGroundedLastFrame;               // 上一帧是否在地面
    private float coyoteTimer;                       // 土狼时间计时器
    #endregion

    #region === Unity 生命周期 ===
    protected virtual void Awake()
    {
        // 获取并缓存必要组件
        sr = GetComponent<SpriteRenderer>();
        fx = GetComponent<EntityFX>();
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<CharacterStats>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {

    }

    private void FixedUpdate()
    {
        // 每帧固定物理检测地面状态
        UpdateGroundCheck();
    }
    #endregion

    #region === 地面检测 ===
    private void UpdateGroundCheck()
    {
        // 使用 OverlapCircle 检测地面（比 Raycast 更稳定）
        bool hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckDistance, whatIsGround);

        if (hit)
        {
            // 检测到地面 → 立即重置土狼计时
            isGrounded = true;
            coyoteTimer = coyoteTime;
        }
        else
        {
            // 未检测到 → 递减土狼计时器
            coyoteTimer -= Time.fixedDeltaTime;

            // 只要土狼时间没耗尽，仍视为在地面上（防止落地瞬间切回空中状态）
            isGrounded = coyoteTimer > 0f;
        }

        // 记录上一帧的状态（方便后续逻辑比较）
        wasGroundedLastFrame = isGrounded;
    }
    public bool CheckSlope() // 是否在斜坡上
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);

        if (hit.collider != null)
        {
            // 获取法线
            slopeNormal = hit.normal;

            // 与竖直方向的夹角（单位：度）
            slopeAngle = Vector2.Angle(slopeNormal, Vector2.up);

            // 判断是否算斜坡（可自定义阈值，比如大于 12° 认为是斜坡）
            isOnSlope = slopeAngle > 12f;

            return true;
        }

        // 没检测到地面
        isOnSlope = false;
        slopeAngle = 0f;
        return false;
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        UnityEditor.Handles.DrawWireDisc(groundCheck.position, Vector3.forward, groundCheckDistance);
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
    }  
#endif

    
    #endregion

    #region === 翻转功能 ===
    /// <summary>
    /// 执行角色翻转（改变朝向）
    /// </summary>
    public virtual void Flip(GameObject gameObject)
    {
        facingDer *= -1;           // 翻转方向标志
        facingRight = !facingRight; // 翻转朝向状态
        gameObject.transform.Rotate(0, 180, 0); // 实际旋转角色
        onFlipped?.Invoke();        // 通知其他系统（例如武器朝向、UI镜像）
    }

    /// <summary>
    /// 根据输入自动控制翻转
    /// </summary>
    public virtual void FlipContorller(float x, GameObject gameObject)
    {
        // 向右输入但当前朝左 → 翻转
        if (x > 0 && !facingRight)
            Flip(gameObject);
        // 向左输入但当前朝右 → 翻转
        else if (x < 0 && facingRight)
            Flip(gameObject);
    }
    #endregion

    #region === 速度控制 ===
    /// <summary>
    /// 将速度清零
    /// </summary>
    public virtual void zeroVelocity()
    {
        SetVelocity(0, 0);
    }

    /// <summary>
    /// 设置刚体速度，并自动执行翻转检测
    /// </summary>
    public virtual void SetVelocity(float xVelocity, float yVelocity)
    {
        if (isKnockback) // 如果处于击退中则不响应输入
            return;

        // 自动根据移动方向翻转角色
        FlipContorller(xVelocity, cameraFollow);

        // 设置 Rigidbody2D 的速度
        rb.velocity = new Vector2(xVelocity, yVelocity);
    }
    #endregion

    #region === 受击与击退逻辑 ===
    /// <summary>
    /// 受到伤害时触发的受击效果
    /// </summary>
    public virtual void DamageImpact()
    {
        fx.StartCoroutine("FlahFX"); // 触发闪烁特效
        StartCoroutine("HitKnockback"); // 开启击退协程
    }

    /// <summary>
    /// 计算击退方向（根据伤害来源）
    /// </summary>
    public virtual void SetKnockbackDir(Transform _DamageDir)
    {
        // 如果伤害来源在右侧 → 击退向左 (-1)
        knockbackDer = _DamageDir.position.x > transform.position.x ? -1 : 1;
    }

    /// <summary>
    /// 击退逻辑协程
    /// </summary>
    public virtual IEnumerator HitKnockback()
    {
        isKnockback = true; // 标记进入击退状态，禁止玩家控制

        // 应用击退速度
        rb.velocity = new Vector2(knockbackPower.x * knockbackDer, knockbackPower.y);

        // 等待击退时间
        yield return new WaitForSeconds(knockbackDuration);

        // 恢复正常状态
        isKnockback = false;
        zeroVelocity(); // 停止移动
    }
    private float slopeAngle = 0f;   // 当前地面角度
    private bool isOnSlope = false;  // 是否在斜坡上
    private Vector2 slopeNormal;     // 地面法线

    
    #endregion
}
