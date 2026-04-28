using HSM;
using UnityEngine;

namespace qwq
{
    public class IceWeapon : Weapon
    {
        [Header("子弹")]
        public GameObject bulletPrefab;  // 子弹预制体

        [Header("参数设置")]
        public float windupTimer;  // 前摇计时器
        public float maxWindup = 0.2f;  // 最大前摇时间
        float cooldownTimer;  // 冷却计时器
        public float maxCooldown = 0.1f;  // 最大冷却时间
        public bool canAttack;  // 攻击许可

        public float spawnRadius = 1;  // 子弹生成半径
        public Vector2 spawnOffset;  // 子弹生成偏移

        private void Update()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                canAttack = false;
            }
            else
            {
                canAttack = true;
            }
        }
        public override bool IsEnter() => canAttack;

        public override void OnEnter()
        {
            windupTimer = 0;
            ctx.anim.SetAnim(ctx.anim.rangedAttackAnim, false).MixDuration = 0.05f;
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            // 如果还在冷却中，直接返回
            if (cooldownTimer > 0) return;

            // 在攻击时停止移动
            ctx.velocity = Vector2.zero;

            // 检查攻击计时是否完成
            if (windupTimer > maxWindup)
            {
                // 检查能量是否足够
                if (!ctx.EnergyValueUPdate(-2)) return;  // 能量不足，无法攻击

                windupTimer = 0; // 重置计时器

                // 计算子弹生成位置
                Vector2 bulletPosition = ctx.mouseWorldPos_8 * spawnRadius + spawnOffset + (Vector2)ctx.transform.position;
                // 计算子弹的朝向角度（基于鼠标位置）
                float bulletAngle = Mathf.Atan2(ctx.mouseWorldPos_8.y, ctx.mouseWorldPos_8.x) * Mathf.Rad2Deg;

                // 实例化子弹
                GameObject newBullet = Instantiate(bulletPrefab, bulletPosition, Quaternion.Euler(0, 0, bulletAngle));

                // 设置子弹的飞行方向
                newBullet.GetComponent<Bullet>().direction = ctx.mouseWorldPos_8;

                // 进入冷却状态
                cooldownTimer = maxCooldown;
                canAttack = false;
            }
            else
            {
                // 更新攻击计时
                windupTimer += deltaTime;
            }
        }
    }
}