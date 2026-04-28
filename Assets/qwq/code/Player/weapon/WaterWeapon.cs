using HSM;
using qwq;
using Unity.VisualScripting;
using UnityEngine;

namespace qwq
{
    public class WaterWeapon : Weapon
    {
        [Header("子弹")]
        public GameObject bulletPrefab;  // 子弹预制体

        [Header("参数设置")]
        float windupTimer;  // 前摇计时
        public float maxWindup = 0.15f;  // 最大前摇
        float cooldownTimer;  // 冷却计时
        public float maxCooldown = 0.1f;  // 最大冷却
        public bool canAttack;  // 攻击许可

        public float spawnRadius = 1;  // 生成半径
        public Vector2 spawnOffset;  // 生成偏移

        private void Awake()
        {
          
        }
        private void Update()
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
       
            }
            else
            {
                canAttack = true;
            }
        }

        public override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.rangedAttackAnim, false).MixDuration = 0.02f;
            canAttack = true;
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            if (cooldownTimer > 0) return;

            ctx.velocity = Vector2.zero;
            if (windupTimer > maxWindup)
            {
                if (!ctx.EnergyValueUPdate(-1)) return;  // 能量不足，无法攻击

                windupTimer = 0;
                Vector2 Bullet_position;
                Bullet_position = ctx.mouseWorldPos_8 * spawnRadius + spawnOffset + (Vector2)ctx.transform.position;
                float angle = Mathf.Atan2(ctx.mouseWorldPos_8.y, ctx.mouseWorldPos_8.x) * Mathf.Rad2Deg;

                GameObject newBullet = UnityEngine.Object.Instantiate(bulletPrefab, Bullet_position, Quaternion.Euler(0, 0, angle));
                newBullet.GetComponent<Bullet>().direction = ctx.mouseWorldPos_8;

                cooldownTimer = maxCooldown;
                canAttack = false;
            }
            else
            {
                windupTimer += deltaTime;
            }

        }

        public override bool IsEnter()
        {
            return canAttack;
        }
    }
}
