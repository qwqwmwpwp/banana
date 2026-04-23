using UnityEngine;

namespace Enemy.Boss.TailKing_Boss_3_.State
{
    public class IdleState : StateBase
    {
        private TailKingController controller;
        private float attackDistance = 2f;    // 攻击距离（与ChaseState的retreatDistance一致）
        private float chaseDistance = 4f;       // 开始追逐的距离（与ChaseState一致）
        private Transform target;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (TailKingController)owner;
        }

        public override void OnEnter()
        {
            controller.SetAnim(controller.idle, true);
            controller.rb.velocity = Vector2.zero;
            target = PlayerManager.instance.player.transform;
        }

        public override void OnUpdate()
        {
            float distanceToPlayer = Mathf.Abs(controller.characterPosition.position.x - target.position.x);
            bool canUseNormalChop = controller.skillInfo[TailKingType.NormalChop].cd;
            bool canUseOtherSkill = controller.skillInfo[TailKingType.TripleChop].cd; 
            
            bool shouldRetreat = (!controller.TailKingstatus.GetShield() && !canUseNormalChop) ||
                                 (controller.TailKingstatus.GetShield() && !canUseNormalChop && !canUseOtherSkill);
            if (!shouldRetreat && distanceToPlayer > chaseDistance)
            {
                controller.ChangeState(TailKingType.ChaseState);
                return;
            }
            // 检查是否在攻击范围内
            if (distanceToPlayer <= attackDistance)
            {
                TryAttack();
            }
           
        }

        private void TryAttack()
        {
            
            // 有护盾时的技能优先级
            if (!controller.TailKingstatus.GetShield())
            {
                if (UnleashSkill(TailKingType.TripleChop)) 
                    return;
                if (UnleashSkill(TailKingType.NormalChop))
                    return;
            }
            else
            { 
                // 无护盾时只使用普通攻击
                if (UnleashSkill(TailKingType.NormalChop))
                    return;
            }
        }

        private bool UnleashSkill(TailKingType skill)
        {
            if (controller.skillInfo == null || !controller.skillInfo.ContainsKey(skill))
                return false;
                
            SkillInfo s = controller.skillInfo[skill];
            if (s != null && !s.cd)
            {
                s.cd = true;
                controller.ChangeState(skill);
                TimerManager.Instance.TryGetTimer(s.time, () => s.cd = false);
                return true;
            }
            return false;
        }

        public override void OnFixedUpdate()
        {
            
        }

        public override void OnLateUpdate()
        {
          
        }

        public override void OnExit()
        {
           
        }
    }
}