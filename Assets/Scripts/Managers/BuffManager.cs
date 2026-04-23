using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;

public enum BuffType
{
    引燃灯火,
    锋芒之火,
    焦燃之火,
    纯净之火
}
public enum FireType
{
    火种余烛,
    锋芒余烛,
    焦燃余烛
}

public class BuffManager : MonoBehaviour
{
    public static BuffManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void Update()
    {

        fireTimer -= Time.deltaTime;
        buffTimer0 -= Time.deltaTime;
        buffTimer -= Time.deltaTime;

        if (fireTimer < 0)
        {

            foreach (var key in fires.Keys.ToList())
            {
                fires[key] = 0;
            }
        }

        if (buffTimer0 < 0)
        {
            RemoveBuff(BuffType.引燃灯火);
            canAddfire = true;
        }

        // 如果分支buff持续时间归0且在关卡第二阶段
        if (buffTimer < 0 && fireLeveStages == 1)
        {
            UseBuff(BuffType.锋芒之火);
            UseBuff(BuffType.焦燃之火);
            canAddfire = true;
        }


    }

    #region 火BUFF
    // 关卡阶段
    public int fireLeveStages = 0;

    // 掉落火焰预制体
    public GameObject firePrefab;


    // 储存现持有buff的哈希表
    HashSet<BuffType> Buffs = new HashSet<BuffType>();

    // 储存持有对应火焰数量的字典
    public Dictionary<FireType, int> fires = new Dictionary<FireType, int>() 
    {
        {FireType.火种余烛, 0},
        {FireType.锋芒余烛, 0},
        {FireType.焦燃余烛, 0},
    };

    bool canAddfire = true;  //是否可以拾取火焰
    bool canAddfire1 = true; //是否可以拾取锋火
    bool canAddfire2 = true; //是否可以拾取焦火


    int fireDuration = 15;  // 火焰存在时间
    float fireTimer; // 计时器

    // (buff_引燃灯火)的持续时间
    int buffDuration0 = 15; 
    float buffTimer0;

    // 其他buff的持续时间
    int buffDuration = 20;  
    float buffTimer;

    public void AddFire(FireType fireType) // 拾取火焰
    {
        if (fires.TryGetValue(fireType, out var value) && canAddfire)
        {
            AudioManager.instance.PlaySFX(24);


            fires[fireType]++;

            // 刷新持续时间
            fireTimer = fireDuration;
            // 开启或刷新弹窗上的倒计时
            PopupManager.instance.SetFire(fireType, fires[fireType], fireDuration);

            if (fires[fireType] == 5)
            {
                // 生成buff前关闭对应的火焰弹窗
                PopupManager.instance.CloseFire(fireType);

                fires[fireType] = 0;

                if (fireType == FireType.火种余烛)
                {
                    AddBuff(BuffType.引燃灯火); // 添加buff
                    canAddfire = false;
                }

                if (fireType == FireType.锋芒余烛)
                {
                    AddBuff(BuffType.锋芒之火);
                    if (fireLeveStages == 1)
                    {
                        // 如果处于关卡第二阶段，清除另一种火的积累数量
                        fires[FireType.焦燃余烛] = 0; 
                        canAddfire = false;
                    }
                    if (fireLeveStages == 2 ) 
                    {
                        canAddfire1 = false;
                        if (HasBuff(BuffType.焦燃之火))
                        {
                            // 如果处于关卡第三阶段且有另一个buff，合成
                            RemoveBuff(BuffType.锋芒之火);
                            RemoveBuff(BuffType.焦燃之火);

                            AddBuff(BuffType.纯净之火);
                            AudioManager.instance.PlaySFX(25);

                            canAddfire = false;
                        }
                        
                    }
                }
                
                if (fireType == FireType.焦燃余烛)
                {
                    AddBuff(BuffType.焦燃之火);
                    if (fireLeveStages == 1)
                    {
                        fires[FireType.锋芒余烛] = 0;
                        canAddfire = false;
                    }
                    if (fireLeveStages == 2 )
                    {
                        canAddfire2 = false;
                        if(HasBuff(BuffType.锋芒之火))
                        {
                            RemoveBuff(BuffType.锋芒之火);
                            RemoveBuff(BuffType.焦燃之火);

                            AddBuff(BuffType.纯净之火);
                            AudioManager.instance.PlaySFX(25);

                            canAddfire = false;
                        }
                       
                    }
                }
            }
        }
    }

    public void AddBuff(BuffType buffType)// 添加buff
    {
        Buffs.Add(buffType);
        AudioManager.instance.PlaySFX(24);

        if (buffType == BuffType.引燃灯火)
        {
            PopupManager.instance.SetBuff(buffType, buffDuration0);
            buffTimer0 = buffDuration0; // 刷新持续时间
        }
        else 
        {
            PopupManager.instance.SetBuff(buffType, buffDuration);
            buffTimer = buffDuration;
        }
    }

    private void RemoveBuff(BuffType buffType) // 去除buff
    {
        if (HasBuff(buffType)) // 如果该buff存在
        {
            Buffs.Remove(buffType);
            // 关闭对应的buff弹窗
            PopupManager.instance.CloseBuff(buffType);
        }
    }

    public bool HasBuff(BuffType buffType) // 判断是否有该buff
    {
        return Buffs.Contains(buffType);
    }

    public void UseBuff(BuffType buffType)
    {
        RemoveBuff(buffType);
        if (buffType == BuffType.锋芒之火)
            canAddfire1 = true;
        if(buffType==BuffType.焦燃之火)
            canAddfire2 = true;

    }

    public void InstantiateFire(AttackType attackType ,Transform _transform)
    {
        // 掉落火种
        GameObject fire_Buff = Instantiate(firePrefab, _transform.position, Quaternion.identity);

        Fire_BuffController fire = fire_Buff.GetComponent<Fire_BuffController>();

        if (fireLeveStages == 0)
            fire.initialize(FireType.火种余烛);

        if (fireLeveStages == 1)
        {
            if (attackType == AttackType.Melee)
                fire.initialize(FireType.锋芒余烛);
            if (attackType == AttackType.ranged)
                fire.initialize(FireType.焦燃余烛);
        }
    }

    #endregion
}
