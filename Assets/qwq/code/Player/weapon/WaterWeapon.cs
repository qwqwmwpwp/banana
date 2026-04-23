using HSM;
using qwq;
using Unity.VisualScripting;
using UnityEngine;

namespace qwq
{
    public class WaterWeapon : Weapon
    {
        public GameObject Bullet;

        public bool isAttack;
        float t;
        public float t_max = 0.15f;
        float cooling;
        public float Cooling_max = 0.1f;

        public float r = 1;
        public Vector2 displacement;

        private void Awake()
        {
          
        }
        private void Update()
        {
            if (cooling > 0)
            {
                cooling -= Time.deltaTime;
       
            }
            else
            {
                isAttack = true;
            }
        }

        public override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.rangedAttackAnim, true).MixDuration = 0.02f;
            isAttack = true;
        }

        public override void OnExit()
        {

        }

        public override void OnUpdate(float deltaTime)
        {
            if (cooling > 0) return;

            ctx.velocity = Vector2.zero;
            if (t > t_max)
            {
                if (!ctx.EnergyValueUPdate(-1)) return;  // 能量不足，无法攻击

                t = 0;
                Vector2 Bullet_position;
                Bullet_position = ctx.mouseWorldPos_8 * r + displacement + (Vector2)ctx.transform.position;
                float angle = Mathf.Atan2(ctx.mouseWorldPos_8.y, ctx.mouseWorldPos_8.x) * Mathf.Rad2Deg;

                GameObject newBullet = UnityEngine.Object.Instantiate(Bullet, Bullet_position, Quaternion.Euler(0, 0, angle));
                newBullet.GetComponent<Bullet>().direction = ctx.mouseWorldPos_8;

                cooling = Cooling_max;
                isAttack = false;
            }
            else
            {
                t += deltaTime;
            }

        }

        public override bool IsEnter()
        {
            return isAttack;
        }
    }
}
