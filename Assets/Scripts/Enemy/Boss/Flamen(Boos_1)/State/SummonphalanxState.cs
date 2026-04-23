using System.Collections;
using UnityEngine;

namespace Enemy.Boss.State
{
    public class SummonphalanxState:StateBase
    {
        private FlamenController controller;
        private GameObject phalanx;
        private float waitTime = 3f;
        private bool isok = false;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (FlamenController)owner;
        }
        public override void OnEnter()
        {
            
            Debug.Log("进入召唤施法状态");
            AudioManager.instance.PlaySFX(38, controller.character.position);
            controller.SetAnim(controller.Attack1, false,
                () =>
                {
                    phalanx =
                        GameObject. Instantiate(controller.phalanxPrefab,new Vector2(controller.player.transform.position.x, controller.player.transform.position.y-1f),Quaternion.identity);
                    controller.SetAnim(controller.idle, true);
                    TimerManager.Instance.TryGetTimer(waitTime, ()=>isok = true);
                });
          
        }

        public override void OnUpdate()
        {
            if(phalanx==null&& isok) controller.ChangeState(Boss1State.Chase);
        }

        public override void OnFixedUpdate()
        {
        
        }

        public override void OnLateUpdate()
        {
          
        }

        public override void OnExit()
        {
          Debug.Log("退出召唤施法状态");
          isok = false;
        }

       
    }
}