using UnityEngine;

namespace Enemy.Boss.TailKing_Boss_3_.State
{
    public class BackState:StateBase
    {
        private TailKingController controller;
        private Transform target;
        private Rigidbody2D rb2;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (TailKingController)owner;
        }

        public override void OnEnter()
        {
            rb2 = controller.rb;
            target = PlayerManager.instance.player.transform;
            controller.SetAnim(controller.Walk, true);
        }

        public override void OnUpdate()
        {
            if (target == null) return;
            
            float distanceToPlayer = Mathf.Abs(controller.characterPosition.position.x - target.position.x);
            
            // 检查技能CD状态
            bool canUseNormalChop = controller.skillInfo[TailKingType.NormalChop].cd;
            bool canUseOtherSkill = controller.skillInfo[TailKingType.TripleChop].cd; 
            
            bool shouldRetreat = (!controller.TailKingstatus.GetShield() && !canUseNormalChop) ||
                                 (controller.TailKingstatus.GetShield() && !canUseNormalChop && !canUseOtherSkill);

            if (!shouldRetreat)
            {
                //没技能
                rb2.velocity = new Vector2(-1* controller.Direction * 3f, rb2.velocity.y);
            }
            else if(shouldRetreat|| distanceToPlayer > 10f)
            {
                controller.ChangeState(TailKingType.ChaseState);
            }
            
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnLateUpdate()
        {
          
        }

        public override void OnExit()
        {
            // 退出时停止移动
            if (rb2 != null)
            {
                rb2.velocity = new Vector2(0, rb2.velocity.y);
            }
           
        }
    }
}