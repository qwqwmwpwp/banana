using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Enemy.Boss.State
{
    public class SummonMonstersState:StateBase
    {
        private FlamenController controller;
        private Vector2 playerPos;
        private float minDistance = 1f;
        private float maxDistance = 5f;
        private bool isFinished = false;
        public override void Init(IStateMachineOwner owner)
        {
            controller = (FlamenController)owner;
        }

        public override void OnEnter()
        {
           playerPos =(Vector2) controller.player.transform.position;
           isFinished = false;
           //有动画后代替
          controller.SetAnim( controller.Attack1, false,
              () =>
              {
                  CreateMonster();
              });
        }

        public override void OnUpdate()
        {
            
        }

        public void CreateMonster()
        {
            Vector2 randomDirection = GetRandomSemiCircleUp();
            // 随机距离（在minDistance和maxDistance之间）
            float randomDistance = Random.Range(minDistance, maxDistance);
        
            // 计算位置：玩家位置 + 随机方向 * 随机距离
            Vector2 randomPosition = playerPos + randomDirection * randomDistance;
            for(int i = 0;i <2;i++)
            {
                GameObject monster = GameObject.Instantiate(controller.monsterPrefab,randomPosition , Quaternion.identity);
                TimerManager.Instance.TryGetTimer(5f, () =>
                {
                    GameObject.Destroy(monster);
                });  
            }
                
            controller.ChangeState(Boss1State.Chase); 
        }
        /// <summary>
        /// 随机生成0-180度的向量
        /// </summary>
        /// <returns></returns>
        Vector2 GetRandomSemiCircleUp()
        {
            float randomAngle = Random.Range(0f, 180f); // 0到180度
            float rad = randomAngle * Mathf.Deg2Rad;    // 转为弧度
            return new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));
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