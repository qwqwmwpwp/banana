using HSM;
using UnityEngine;

namespace qwq
{
    public class IceWeapon : Weapon
    {
        public GameObject Bullet;

        public float t;
        public float t_max = 0.2f;
        float cooling;
        public float Cooling_max = 0.1f;
        public bool isAttack;

        public float r = 1;
        public Vector2 displacement;

        private void Update()
        {
            if (cooling > 0)
            {
                cooling -= Time.deltaTime;
                isAttack = false;
            }
            else
            {
                isAttack = true;
            }
        }
        public override bool IsEnter() => isAttack;

        public override void OnEnter()
        {
            t = 0;
            ctx.anim.SetAnim(ctx.anim.rangedAttackAnim, true).MixDuration = 0.05f;
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            // 如果还在冷却中，直接返回
            if (cooling > 0) return;

            // 在攻击时停止移动
            ctx.velocity = Vector2.zero;

            // 检查攻击计时是否完成
            if (t > t_max)
            {
                // 检查能量是否足够
                if (!ctx.EnergyValueUPdate(-2)) return;  // 能量不足，无法攻击

                t = 0; // 重置计时器

                // 计算子弹生成位置
                Vector2 bulletPosition = ctx.mouseWorldPos_8 * r + displacement + (Vector2)ctx.transform.position;
                // 计算子弹的朝向角度（基于鼠标位置）
                float bulletAngle = Mathf.Atan2(ctx.mouseWorldPos_8.y, ctx.mouseWorldPos_8.x) * Mathf.Rad2Deg;

                // 实例化子弹
                GameObject newBullet = Instantiate(Bullet, bulletPosition, Quaternion.Euler(0, 0, bulletAngle));

                // 设置子弹的飞行方向
                newBullet.GetComponent<Bullet>().direction = ctx.mouseWorldPos_8;

                // 进入冷却状态
                cooling = Cooling_max;
                isAttack = false;
            }
            else
            {
                // 更新攻击计时
                t += deltaTime;
            }
        }
    }
}