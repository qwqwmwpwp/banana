using System;
using System.Collections;
using System.Collections.Generic;
using Enemy.Boss;
using Enemy.Boss.State;
using Spine;
using Spine.Unity;
using UnityEngine;
[Serializable]
public class SkillInfo
{
    public float probability;
    public float time;
    public bool cd;

    public SkillInfo( float probability, float time, bool cd)
    {
        this.probability = probability;
        this.time = time;
        this.cd = cd;
    }
}
public class FlamenController : EnemyBase, IStateMachineOwner
{
    [Header("动画信息")]
    public SkeletonAnimation animContorller;
    [Header("普通状态动画")]  
    public AnimationReferenceAsset idle; // 锋芒闲置

    public AnimationReferenceAsset Walk;
    [Header("伤害动画")] 
    public AnimationReferenceAsset Attack1;     // 锋芒攻击
    public AnimationReferenceAsset Attack2;     // 焦燃攻击 
       
    [Header("死亡")]
    public AnimationReferenceAsset Die; // 无敌攻击
    [Header("移动速度")]
    public float moveSpeed = 4f;
    [Header("技能设置")]
    public GameObject fireballPrefab; // 火球预制体
    public GameObject strongFirePrefab;
    public GameObject phalanxPrefab; // 法阵预制体
    public GameObject  monsterPrefab;            //怪预制体
    public int Direction;
    public Transform FirePosition;
    public Transform character;
    private Vector2 scale;
    private float FireDis;
    
    private  Dictionary<Boss1State, SkillInfo> rarityWeights = new Dictionary<Boss1State, SkillInfo>()
    {
        { Boss1State.Summonphalanx , new SkillInfo(40,10,true) },
        { Boss1State.SummonMonsters , new SkillInfo(30,10, true)},
        { Boss1State.SummonFireball , new SkillInfo(30,15, true)},
    };
    public Transform player;
    private StateMachine StateMachine = new StateMachine();
   
    public Boss1State currentState;
    [HideInInspector] 
    public Color originalColor;
    private void Start()
    {
        if (animContorller == null)
        {
            Debug.LogError("❌ 未绑定 SkeletonAnimation 组件！");
            return;
        }

        // Spine旧版本没有 initialized 属性，用 this.IsValid() 来判断
        if (animContorller.skeleton == null)
        {
            animContorller.Initialize(true);
        }

        // 防止打包后事件未绑定
        animContorller.AnimationState.Event += OnSpineEvent;
        player = PlayerManager.instance.player.transform;
        StateMachine.Init(this);
       
        // 记录受击前的颜色
        originalColor = animContorller.skeleton.GetColor();
        rb = GetComponent<Rigidbody2D>();
        scale = character.localScale;
        StateMachine.Init(this);
        ChangeState(Boss1State.Chase);
        Direction = 1;   // 右 1 左 -1
        FireDis = Mathf.Abs(FirePosition.position.x - character.position.x);
    }

    private void Update()
    {
        if (player.transform.position.x > character.position.x)
        {
            Direction = 1;
            Filp();
        } 
        else
        {
            Direction = -1;
            Filp();
        }
    }

    private void OnDestroy()
    {
        // 在这里清理资源、取消订阅事件、停止协程等
        StateMachine?.Clear();
        StopAllCoroutines();
    }

    public void Filp()
    {
        character.localScale = new Vector3(scale.x * Direction, scale.y);
        FirePosition.position = new Vector3(character.position.x + Direction *FireDis, FirePosition.position.y);
        
    }
    public void ChangeState(Boss1State state)
    {
        currentState = state;
        switch (state)
        {
            case Boss1State.Chase:
                StateMachine.SwitchState<ChaseState>();
                break;
            case Boss1State.SummonMonsters:
                  StateMachine.SwitchState<SummonMonstersState>();
                  break;
            case Boss1State.Summonphalanx:
                  StateMachine.SwitchState<SummonphalanxState>();
                  break;
            case Boss1State.SummonFireball:
                  StateMachine.SwitchState<SummonFireState>();
                  break;
        }
    }
    public TrackEntry SetAnim(AnimationReferenceAsset _anim, bool _loops)
    {
        // 切换到指定动画，返回 TrackEntry 用于控制动画播放
        TrackEntry entry = animContorller.AnimationState.SetAnimation(0, _anim, _loops);
    
        entry.TrackTime = 0f;      // 重置动画到开头
        entry.MixDuration = 0f;    // 无过渡效果，立即切换
    
        return entry;
    }
    public TrackEntry SetAnim(AnimationReferenceAsset _anim, bool _loops, Action onComplete = null)
    {
        // 切换到指定动画，返回 TrackEntry 用于控制动画播放
        TrackEntry entry = animContorller.AnimationState.SetAnimation(0, _anim, _loops);
    
        entry.TrackTime = 0f;      // 重置动画到开头
        entry.MixDuration = 0f;    // 无过渡效果，立即切换
    
        // 添加动画完成回调
        if (!_loops && onComplete != null)
        {
            entry.Complete += (trackEntry) => 
            {
                onComplete?.Invoke();
            };
        }
    
        return entry;
    }
    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
      
    }
    //根据skillInfo信息随机获取技能
    public  Boss1State GetRandomRarity()
    {
        float totalWeight = 0f;
        foreach (var k in rarityWeights.Values)
        {
            totalWeight += k.probability;
        }
        
        float randomValue = UnityEngine.Random.Range(0f, totalWeight);
        float currentWeight = 0f;
        
        foreach (var kvp in rarityWeights)
        {
            currentWeight += kvp.Value.probability;
            if (randomValue <= currentWeight)
            {
                //在cd就返回
                if (kvp.Value.cd)
                {
                    kvp.Value.cd = false;
                    TimerManager.Instance.TryGetTimer(kvp.Value.time, () =>
                    {
                        kvp.Value.cd = true;
                    });
                    return kvp.Key;
                }
                return Boss1State.Chase;
            }
        }
        //执行不到这
        return Boss1State.Chase;
    }
}
