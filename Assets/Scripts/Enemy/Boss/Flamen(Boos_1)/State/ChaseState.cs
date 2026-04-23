using GameManager;
using UnityEngine;

namespace Enemy.Boss.State
{
    public class ChaseState:StateBase
    {
        private Transform target;
        private float fireCD = 5f;
       //切换状态
        private bool isFire = true;
        private bool isMonster = true;
        private bool isPhalanx = true;
        private bool isStrongFire = true;


        private Rigidbody2D rb;
        private FlamenController controller;
        
        public override void Init(IStateMachineOwner owner)
        {
            controller = (FlamenController)owner;
        }

        public override void OnEnter()
        {
           target = controller.player;
           rb = controller.rb;
           controller.SetAnim(controller.Walk, true);
           isFire = false;
        }

        public override void OnUpdate()
        {
            rb.velocity = new Vector2(controller.Direction * controller.moveSpeed, rb.velocity.y);
           //隔一段时间发射可躲避的火球  
            if (!isFire)
            {
                AudioManager.instance.PlaySFX(42, controller.character.position);
                Fire();
                TimerManager.Instance.TryGetTimer(fireCD, ()=>isFire = true);
            }
            //随机概率切换攻击状态
            controller.ChangeState(controller.GetRandomRarity());
        }
       private void Fire()
       {
           isFire = true;
          int count = Random.Range(1, 3);
          
          for (int i = 0; i < count; i++)
          {
              GameObject fireball = GameObject.Instantiate(controller.fireballPrefab, new Vector2(controller.transform.position.x, controller.transform.position.y+2.5f), Quaternion.identity);
              TimerManager.Instance.TryGetTimer(5f, () =>
              {
                  GameObject.Destroy(fireball);
              });  
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
         
        }
    }
}