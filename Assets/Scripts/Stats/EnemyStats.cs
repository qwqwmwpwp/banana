using System.Collections;
using System.Collections.Generic;
using EnemyCommon;
using UnityEngine;
using UnityEngine.Events;

public class EnemyStats : CharacterStats
{
    protected EnemyBase enemy;
    private PlayerController player;
    
    [Header("�ṩ����")]
    [SerializeField] private int energy;

    public EnemySpawnContext ctx;
    public event UnityAction<EnemySpawnContext> OnDeath;

    protected override void Start()
    {
        base.Start();

        enemy = GetComponent<EnemyBase>();
        player = PlayerManager.instance.player;
    }

    public override void TakeDamage(int _damage, AttackType attackType)
    {
        //若有护甲剩余，伤害为0并扣除一层护甲，否则将根据伤害扣除血量
        _damage = ChackAramor(_damage);

        //更新生命值
        UpdateHealth(_damage);


        if (currrentHealth <= 0 && !isDie)
        {

            BuffManager.instance.InstantiateFire(attackType, transform);

            Die();
            Debug.Log("Die");
        }

        enemy.DamageImpact();

    }

    public virtual void Reset()
    {
        currrentHealth = maxHealth.GetValue();
        currrentArmor = armor.GetValue();
        isDie = false;
    }

    public override void Die()
    {
        base.Die();

        enemy.Dead();
        OnDeath?.Invoke(ctx);
    }
}
