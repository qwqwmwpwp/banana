using UnityEngine;

namespace Enemy.Boss.TailKing_Boss_3_.State
{
    public class NormalChop:StateBase
    {
        private TailKingController controller;
        private AnimatorStateInfo stateInfo;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (TailKingController)owner;
        }

        public override void OnEnter()
        {
           controller.SetAnim(controller.Attack1, false,
               (() => controller.ChangeState(TailKingType.Idle))
               );
        }

        public override void OnUpdate()
        {
                    //砍完进入追逐
               // controller.ChangeState(TailKingType.ChaseState);
            
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