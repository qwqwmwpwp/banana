using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballTracking : MonoBehaviour
{
    public float speed = 5f;
    private Transform target;
    public float harm;

  
    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        
          
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<CharacterStats>() != null)
            {
                
                //取得攻击对象的 Stats 
                CharacterStats targetStats = other.GetComponent<CharacterStats>();
                targetStats.TakeDamage((int)harm,AttackType.ranged);

            }
            Destroy(gameObject);
        }
    }
  
    private void Update()
    {
        Tracking(target);
    }

    void Tracking(Transform target)
    {
        // 平滑移动到目标位置
        transform.position = Vector3.MoveTowards(
            transform.position, 
            target.position, 
            speed * Time.deltaTime
        );
    }
}
