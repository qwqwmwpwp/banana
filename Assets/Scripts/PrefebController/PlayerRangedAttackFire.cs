using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerRangedAttackFire : MonoBehaviour
{
    public float speed = 10f;
    private int direction = 1; // 1 = 右，-1 = 左
    private float timer = 3;
    private Animator anim;
    private bool canMove;
    private bool isCharge;

    // 初始化
    public void SetDirection(bool facingRight , bool _isCharge)
    {
        canMove = true;
        direction = facingRight ? 1 : -1;
        Vector3 scale = transform.localScale;

        if(facingRight)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
            transform.localScale = scale;
        }

        anim = GetComponent<Animator>();
        isCharge = _isCharge;
    }

    private void FixedUpdate()
    {
        if (!canMove)
            return;

        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            DestroyMy();
        }
    }

    private void TransformFreeze() => canMove = false;
    private void DestroyMy() => Destroy(gameObject);
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats targetStats = collision.GetComponent<EnemyStats>();
        if (targetStats == null) targetStats = collision.GetComponentInParent<EnemyStats>();
        if (targetStats != null)
        {
            anim.SetTrigger("Explosion");

            AudioManager.instance.PlaySFX(14);


            //取得攻击对象的 Stats 
          
            float attackMultiplier;

            if (isCharge)
                attackMultiplier = 2.5f;
            else
                attackMultiplier = 1;

            PlayerManager.instance.player.playerStats.DoDamage(targetStats, AttackType.ranged, attackMultiplier);

            Debug.Log(targetStats.currrentHealth);
        }
    }


}
