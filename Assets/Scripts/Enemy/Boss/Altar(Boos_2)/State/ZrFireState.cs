using UnityEngine;

namespace Enemy.Boss.Altar_Boos_2_.State
{
    public class ZrFireState:StateBase
    {
        AltarController controller;
   
        public override void Init(IStateMachineOwner owner)
        {
            controller = owner as AltarController;
        }

        public override void OnEnter()
        {
            //ToDo:播放动画
            controller.SetAnim(controller.ZrIdleAnim, true);
        }

        public override void OnUpdate()
        {
          
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