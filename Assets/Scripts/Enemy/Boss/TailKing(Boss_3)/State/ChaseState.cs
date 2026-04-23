using UnityEngine;

namespace Enemy.Boss.TailKing_Boss_3_.State
{
    public class ChaseState : StateBase
    {
        private TailKingController controller;
        private Rigidbody2D rb1;
        private Transform target;
        
        // 距离参数
        private float retreatDistance = 2.5f;  // 后退的距离阈值
        private float chaseDistance = 4f;      // 重新追击的距离阈值
        private bool backOff;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (TailKingController)owner;
        }

        public override void OnEnter()
        {
           // Debug.Log("进入追逐状态");
            rb1 = controller.rb;
            target = PlayerManager.instance.player.transform;
            controller.SetAnim(controller.Walk, true);
        }

        public override void OnUpdate()
        {
            
            // 没有目标就退出
            if (target == null) return;
            
            float distanceToPlayer = Mathf.Abs(controller.characterPosition.position.x - target.position.x);
            
            // 检查技能CD状态
            bool canUseNormalChop = controller.skillInfo[TailKingType.NormalChop].cd;
            bool canUseOtherSkill = controller.skillInfo[TailKingType.TripleChop].cd; 
            
            bool shouldRetreat = (!controller.TailKingstatus.GetShield() && !canUseNormalChop) ||
                                 (controller.TailKingstatus.GetShield() && !canUseNormalChop && !canUseOtherSkill);

            if (distanceToPlayer > 7f )
            {
                rb1.velocity = new Vector2(controller.Direction * controller.current_speed, rb1.velocity.y);
            }
            else
            {
                //有技能释放
                if (shouldRetreat)
                {
                    if (distanceToPlayer <= 2f)
                    {
                      //  Debug.Log("准备待机攻击");
                        controller.ChangeState(TailKingType.Idle);
                    }
                    else
                    {
                        rb1.velocity = new Vector2(controller.Direction * controller.current_speed, rb1.velocity.y);
                    }
                }
                else
                {
                    
                    if (distanceToPlayer <= 3f)
                    {
                        controller.ChangeState(TailKingType.BackState);
                    }
                    else 
                    {
                        rb1.velocity = new Vector2(controller.Direction * controller.current_speed, rb1.velocity.y);
                    }
                  
                }
                
            }
            
        }

        public override void OnFixedUpdate() { }
        public override void OnLateUpdate() { }
        public override void OnExit() 
        {
            // 退出时停止移动
            if (rb1 != null)
            {
                rb1.velocity = new Vector2(0, rb1.velocity.y);
            }
          
        }
    }
}