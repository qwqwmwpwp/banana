using DG.Tweening;
using Spine.Unity;
using UnityEngine;

namespace Enemy.Boss.Flamen_Boos_1_.Status
{
    public class FlamenStatus:EnemyStats
    {
        [Header("击退参数")]
        public Vector2 knockbackPower;
        [Header("灯台")]
        public Lighthouse light;
        private FlamenController controller;
        private bool isHit;
        [SerializeField]
        private GameObject UI_healthBar;
        [SerializeField]
        private GameObject Shield;
        [SerializeField]private bool isShield;
       [Header("空气墙")]
        public AirwallTrigger airwall;
        protected override void Start()
        {
            base.Start(); 
            controller = GetComponent<FlamenController>();
            isHit = false;
            isShield = true;
            Shield = UI_healthBar.transform.GetChild(0).gameObject;
        }
        public override void TakeDamage(int _damage, AttackType attackType)
        {
            //拥有buff就破盾
            if (light.isKindled)
            {
                isShield = false;
                Shield.SetActive(false);
            }
            //判断是否破盾或受伤期间
            if(isShield || isHit) return;
             PlayFlash();
             enemy.rb.velocity = Vector2.zero;
             enemy.rb.AddForce(new Vector2(knockbackPower.x *-1* controller.Direction, knockbackPower.y), ForceMode2D.Impulse);
            //更新生命值
            UpdateHealth(_damage);
            if (currrentHealth <= 0 && !isDie)
            {
                Die();
                Debug.Log("Die");
            }

            

        }
        public void SetShield(bool value)
        {
            isShield = value;
        }
        public  bool GetShield()
        {
            return isShield;
        }
        public override void Die()
        {
            AudioManager.instance.ChangeBGM(0);
            UI_healthBar.SetActive(false);
            AudioManager.instance.PlaySFX(39, controller.character.position);
            airwall.StopWall();
            controller.SetAnim(controller.Die, false, () => { Destroy(gameObject); });
        }
        [SerializeField] private Color flashColor = Color.red; // 闪烁颜色
        [SerializeField] private int flashCount = 3;           // 闪烁次数
        [SerializeField] private float flashDuration = 0.1f;   // 每次闪烁时长


        private void PlayFlash()
        {
            // 先停止之前的闪烁，避免叠加
            DOTween.Kill(controller.animContorller.skeleton);
            /// <summary>
            /// SetLoops（重复次数, SetLoops）：
            /// 
            /// 【LoopType.Restart】
            /// 每次都从 起点 开始重新播放。
            /// A → B, A → B
            /// 【LoopType.Yoyo】
            /// 正向一次，反向一次
            /// A → B, B → A
            /// 【LoopType.Incremental】
            /// 每次循环，都会在目标值上累加。
            /// A → B, B → C, C → D...
            /// </summary>
           controller.animContorller.skeleton
                .DOColor(flashColor, flashDuration)      // 变成闪烁颜色(一次动画)
                .SetLoops(flashCount * 2, LoopType.Yoyo) // 对 DOColor 返回的动画做循环操作
                .OnStart(() =>
                {
                    // 动画开始时确保是无敌状态
                    isHit = true;
                })
                .OnComplete(() =>
                {
                    // 当 Tween 正常完成所有循环并到达终点 时触发一次回调，闪烁结束恢复原始颜色
                    isHit = false;
                   controller.animContorller.skeleton.SetColor(controller.originalColor);
                })
                .OnKill(() =>
                {
                    // 如果 tween 被提前 Kill 时触发一次回调，确保恢复颜色
                    if (controller != null &&controller.animContorller != null)
                       controller.animContorller.skeleton.SetColor(controller.originalColor);
                });

        }
    }
}