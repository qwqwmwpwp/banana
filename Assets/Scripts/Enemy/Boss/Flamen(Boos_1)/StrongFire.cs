using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 一直跟踪
    /// </summary>
    public class StrongFire:MonoBehaviour
    {
        [Header("速度")]
        public float moveSpeed = 3f;
        [Header("伤害")]
        public int damage = 10;
        private Transform _target;

        private void Start()
        {
            _target = PlayerManager.instance.player.transform;
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.position, moveSpeed * Time.deltaTime);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                PlayerStats P = other.gameObject.GetComponent<PlayerStats>();
                if (P != null)
                {
                    P.TakeDamage(damage,AttackType.ranged);
                    Destroy(gameObject);
                }
                  
            }
        }
    }
}