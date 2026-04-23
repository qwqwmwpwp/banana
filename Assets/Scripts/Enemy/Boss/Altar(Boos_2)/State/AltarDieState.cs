using UnityEngine;

namespace Enemy.Boss.Altar_Boos_2_.State
{
    public class AltarDieState:StateBase
    {
        AltarController controller;
   
        public override void Init(IStateMachineOwner owner)
        {
            controller = owner as AltarController;
        }

        public override void OnEnter()
        {
            controller.SetAnim(controller.Die, true);
            AudioManager.instance.ChangeBGM(0);
            controller.UI_healthBar.SetActive(false);
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