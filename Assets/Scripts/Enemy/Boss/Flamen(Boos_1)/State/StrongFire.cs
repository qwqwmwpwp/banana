using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy.Boss.State
{
    public class StrongFire:MonoBehaviour
    {
        [Header("伤害")]
        public int damage;
        private Transform target;
        public float moveSpeed = 3f;
        private Rigidbody2D rb;
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log("进入触发器");
            if (other.CompareTag("Player"))
            {
                Debug.Log("玩家进入检测区域");
                PlayerStats stats = other.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    stats.TakeDamage(damage, AttackType.Melee);
                }
            }
            Destroy(gameObject);
        }
        private void Start()
        {
            rb  = GetComponent<Rigidbody2D>();
            target = PlayerManager.instance.player.transform;
            StartCoroutine(DestroyAfterDelay(10f));
        }

        private void Update()
        {
            ChasePlayer();
        }
        void ChasePlayer()
        {
            // 计算朝向玩家的方向
            Vector2 direction = (target.position - transform.position).normalized;
        
            // 计算目标位置
            Vector2 targetPosition = (Vector2)transform.position + direction * moveSpeed * Time.fixedDeltaTime;
        
            // 使用 MovePosition 平滑移动
            rb.MovePosition(targetPosition);
        
         
        }
        private IEnumerator DestroyAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(gameObject);
        }
    }
}