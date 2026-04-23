using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public enum StatType
{
    maxHealth,     //最大生命
    armor          //护甲
}
public enum AttackType
{
    Melee ,  // 近战
    ranged   // 远程
}
public class CharacterStats : MonoBehaviour
{

    public bool isDie = false;


    [Header("主要属性")]
    public int currrentHealth;//当前血量
    public int currrentArmor; //当前护甲
    public Stat damage;       //基础伤害


    [Header("默认属性")]
    public Stat maxHealth;    //最大生命
    public Stat armor;        //最大护甲

    public System.Action onHealthChanged;//生命值更改时的委托，用于后续UI血条更新


    private void Awake()
    {
        //maxHealth.SetValue(100);
    }

    protected virtual void Start()
    {
        currrentHealth = maxHealth.GetValue();
        UpdateHealth(0);//更新生命值

        currrentArmor = armor.GetValue();

    }

    protected virtual void Update()
    {
        
    }


    public virtual void DoDamage(CharacterStats _targetStats, AttackType attackType, float AttackMultiplier)//判断护盾并计算伤害
    {

        int totalDamage = damage.GetValue();

        totalDamage = (int)(totalDamage * AttackMultiplier);
    

        //预留逻辑

       Debug.Log("主角攻击:"+ totalDamage);

        _targetStats.TakeDamage(totalDamage, attackType);

    }



  public virtual void TakeDamage(int _damage, AttackType attackType)
    {
        //若有护甲剩余，伤害为0并扣除一层护甲，否则将根据伤害扣除血量
        _damage = ChackAramor(_damage);

        //更新生命值
        UpdateHealth(_damage);

        if (currrentHealth <= 0 && !isDie)
        {
            Die();
            Debug.Log("Die");
        }
    }

    public virtual void TakeDamageWithNoAramor(int _damage)
    {
        //更新生命值
        UpdateHealth(_damage);


        if (currrentHealth <= 0 && !isDie)
        {
            Die();
            isDie = true;
        }
    }

    protected int ChackAramor(int totalDamage)//计算目标的护甲
    {
        if (currrentArmor > 0)
        {
            totalDamage = 0;
            
            currrentArmor--;

            AudioManager.instance.PlaySFX(15);

        }
        else
        {
            AudioManager.instance.PlaySFX(16);

        }
        return totalDamage;
    }



    public virtual void UpdateHealth(int _damage)//受伤更新生命值
    {
        currrentHealth -= _damage;
        onHealthChanged?.Invoke();
    }

    public virtual void RestoreHealth(float _percentage)//比例恢复生命值
    {
        currrentHealth += (int)(maxHealth.GetValue() * _percentage);

        if (currrentHealth > maxHealth.GetValue())
            currrentHealth = maxHealth.GetValue();

        onHealthChanged?.Invoke();
    }
    public virtual void FullyRestoreArmor() // 回复全部护甲的方法
    {
        currrentArmor = armor.GetValue();
        UIManager.instance.GameUI.UpdateArmorUI();

    }

    public virtual void Die()
    {
        if (isDie) return; // 避免重复调用
        isDie = true;
    }


}







