using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fireball : MonoBehaviour
{
    PlayerController player;

    [SerializeField] private CharacterStats targetStat;//目标属性
    [SerializeField] private float speed;              //速度
    [SerializeField] private float attackCheckRadius;  //攻击半径
    [SerializeField] private float existenceTime;      //存在时间
    private int damage;
    private Vector3 direction;

    void Start()
    {
        player = PlayerManager.instance.player;

    }

    void Update()
    {
        //沿着传入的方向直线前进，与 player 的距离小于一定值时，调用 SelfDestructing()
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(player.transform.position, transform.position) < 0.1f)
        {
            AttackTrigger();
        }

        //如果存在时间已经小于零，销毁自身
        existenceTime -= Time.deltaTime;
        if (existenceTime < 0)
        {
            Destroy(gameObject);
        }
    }

    public void Setup(int _damage, CharacterStats _targetStat, Vector3 _direction)//初始化
    {
        damage = _damage;
        targetStat = _targetStat;
        direction = _direction;
    }

    public void AttackTrigger()
    {
        //创造一个临时对撞机，利用 Physics2D.OverlapCircleAll 创造重叠圆填满它
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<PlayerController>() != null)
            {
                //取得攻击对象的 Stats 
                PlayerStats targetStats = hit.GetComponent<PlayerStats>();

                //调用人物受击函数
                targetStats.TakeDamage(damage,AttackType.ranged);

                //预留动画

                //销毁自身
                Destroy(gameObject);

            }
        }
    }
}
