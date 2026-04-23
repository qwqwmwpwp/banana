using EnemyCommon;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

/// <summary>
/// 【敌人基础类】
/// 所有敌人继承的基础行为：击退、受击、死亡基础流程。
/// </summary>
public enum SpawnType
{
    场景预设怪物,
    刷怪器生成
}

public class EnemyBase : MonoBehaviour
{
    public EnemyTypes type;

    [Header("来源类型")]
    public SpawnType spawnType = SpawnType.场景预设怪物;
    public void Init(SpawnType type)
    {
        spawnType = type;
        stats.Reset();
    }

    [Header("击退信息")]
    [SerializeField] protected Vector2 knockbackPower;   // 击退的力度（x = 横向力量，y = 纵向力量）
    [SerializeField] protected float knockbackDuration;  // 击退持续时间
    protected bool isKnockback;                         // 当前是否处于击退状态

    public Rigidbody2D rb;     // 敌人的物理组件
    public EntityFX fx;        // 控制受击时闪白等特效的组件
    public EnemyStats stats;
    /// <summary>
    /// 初始化基础组件。
    /// 从子物体中获取 Rigidbody2D 和 FX（有些模型可能挂在子节点）。
    /// </summary>
    protected virtual void Awake()
    {
        stats = GetComponentInChildren<EnemyStats>();
        rb = GetComponentInChildren<Rigidbody2D>();    // 从子物体查找刚体
        fx = GetComponentInChildren<EntityFX>();       // 从子物体查找受击特效控制器
    }

    /// <summary>
    /// 击退方向（1 = 右，-1 = 左）
    /// 由攻击者位置自动计算
    /// </summary>
    public int knockbackDer { get; private set; }

    /// <summary>
    /// 敌人受到伤害后的基础反馈：
    /// 1. 播放受击特效（闪白）
    /// 2. 执行击退协程
    /// </summary>
    public virtual void DamageImpact()
    {
        // 如果已被禁用（对象池中），不执行任何逻辑
        if (!gameObject.activeSelf) return;

        fx.StartCoroutine("FlahFX");      // 播放受击闪白
        StartCoroutine("HitKnockback");   // 执行击退
    }

    /// <summary>
    /// 根据攻击者位置计算击退方向
    /// 攻击者在右 → 向左击退（-1）
    /// 攻击者在左 → 向右击退（1）
    /// </summary>
    public virtual void SetKnockbackDir(Transform _DamageDir)
    {
        if (_DamageDir.position.x > transform.position.x)
            knockbackDer = -1;  // 攻击来自右侧 → 击退向左
        else
            knockbackDer = 1;   // 攻击来自左侧 → 击退向右
    }

    /// <summary>
    /// 敌人击退协程：
    /// 1. 开始击退：给 rigidbody 一个击退速度
    /// 2. 等待 knockbackDuration 秒
    /// 3. 结束击退
    /// </summary>
    public virtual IEnumerator HitKnockback()
    {
        isKnockback = true;

        // 设置击退速度（方向 × 力度）
        rb.velocity = new Vector2(knockbackPower.x * knockbackDer, knockbackPower.y);

        // 持续击退 knockbackDuration 秒
        yield return new WaitForSeconds(knockbackDuration);

        isKnockback = false;
    }

    /// <summary>
    /// 敌人初始化入口，用于：
    /// - 对象池复用时重置状态
    /// - 根据 EnemySpawnContext 设置初始属性
    /// - 子类可以重写用于 AI 初始化
    /// </summary>
    public virtual void Init(EnemySpawnContext ctx)
    {
    }

    /// <summary>
    /// 敌人死亡逻辑入口
    /// 子类重写实现：播放死亡动画、掉落物品、回收对象等
    /// </summary>
    public virtual void Dead()
    {
        if (spawnType == SpawnType.场景预设怪物)
        {
            // 直接销毁
            Destroy(gameObject);
        }
        else
        {
            // 回收到对象池
            EnemySpawnManager.instance.ReturnEnemy(type, gameObject);
        }
        
    }
}
