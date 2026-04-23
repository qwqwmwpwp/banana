using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy.Boss.State
{
    public class SummonFireState:StateBase
    {
        private FlamenController _controller;
        [Header("火球数量")]
        private int _fireballCount = 3;
        [Header("火球间隔")]
        private float _fireballInterval = 0.8f;
        public override void Init(IStateMachineOwner owner)
        {
            _controller = owner as FlamenController;
        }
       
        public override void OnEnter()
        {
            AudioManager.instance.PlaySFX(40, _controller.character.position);
            _controller.SetAnim(_controller.Attack2, false,
                () =>
                {
                    AudioManager.instance.PlaySFX(41, _controller.character.position);
                    _controller.StartCoroutine(StartFire());
                    _controller.ChangeState(Boss1State.Chase);
                } );
            
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
        IEnumerator StartFire()
        {
            for (int i = 0; i < _fireballCount; i++)
            {
                GameObject.Instantiate(_controller.strongFirePrefab,
                    _controller.FirePosition.position,
                    Quaternion.identity);
                yield return new WaitForSeconds(_fireballInterval);
            }
        }
    }
}