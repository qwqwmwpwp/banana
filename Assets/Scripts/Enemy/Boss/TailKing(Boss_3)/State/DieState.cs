namespace Enemy.Boss.TailKing_Boss_3_.State
{
    public class DieState:StateBase
    {
        private TailKingController controller;

        public override void Init(IStateMachineOwner owner)
        {
            controller = (TailKingController)owner;
        }

        public override void OnEnter()
        {
            
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