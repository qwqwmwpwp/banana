using System;
using System.Collections.Generic;
using Enemy.Boss.Altar_Boos_2_.State;
using Enemy.Boss.Altar_Boos_2_.Status;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Enemy.Boss.Altar_Boos_2_
{
    [Serializable]
    public class AltarStateInfo
    {
        public float probability;
        public float time;

        public AltarStateInfo( float probability, float time)
        {
            this.probability = probability;
            this.time = time;
        }
    }
    public class AltarController : EnemyBase,IStateMachineOwner
    {
        
        [Header("动画信息")]
        public SkeletonAnimation animContorller;
        [Header("普通状态动画")]  
        public AnimationReferenceAsset FmIdleAnim; // 锋芒闲置
        public AnimationReferenceAsset ZrIdleAnim; // 焦燃闲置
        public AnimationReferenceAsset InvincibleAnim; // 无敌闲置
        [Header("伤害")] 
        public AnimationReferenceAsset FmAttack;     // 锋芒攻击
        public AnimationReferenceAsset ZrAttack;     // 焦燃攻击 
        public AnimationReferenceAsset InvincibleAttack; // 无敌攻击
        [Header("死亡")]
        public AnimationReferenceAsset Die; // 无敌攻击
        [Header("血条")]
        public GameObject UI_healthBar;
        private StateMachine StateMachine = new StateMachine();
        [Header("空气墙")]
        public AirwallTrigger airwall;
        [HideInInspector]  public AltarStatus stats;
        public AltarStateType CurrentState
        {
            get
            {
                return currentState;
            }
        }
        [SerializeField] private AltarStateType currentState;
        private  Dictionary<AltarStateType, AltarStateInfo> rarityWeights =new Dictionary<AltarStateType, AltarStateInfo>()
        {
            {AltarStateType.Invincible, new AltarStateInfo(0.2f, 8f)},
            {AltarStateType.FmFire, new AltarStateInfo(0.4f, 30f)},
            {AltarStateType.ZrFire, new AltarStateInfo(0.4f, 30f)}
        };

        protected override void Awake()
        {
            base.Awake();
        }

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
            StateMachine.Init(this);
            stats = GetComponent<AltarStatus>();
            ChangeState(GetRandomRarity());
            
        }

        private void Update()
        {
            // if(Input.GetKeyDown(KeyCode.K))
            // {
            //     foreach (KeyValuePair<AltarStateType, AltarStateInfo> v in rarityWeights)
            //     {
            //         Debug.Log($"Rarity: {v.Key} | Probability: {v.Value.probability} | Time: {v.Value.time}");
            //     }
            // }
        }

        public void ChangeState(AltarStateType state)
        {
            if (rarityWeights.Count == 1)
            {
                airwall.StopWall();
                StateMachine.SwitchState<AltarDieState>();
                return; 
            }
                
            switch (state)
            {
                case AltarStateType.Invincible:
                    StateMachine.SwitchState<InvincibleState>();
                    TimerManager.Instance.TryGetTimer(rarityWeights[state].time, () =>
                    {
                         ChangeState(GetRandomRarity());
                    });
                    break;
                case AltarStateType.FmFire: 
                    StateMachine.SwitchState<FmFireState>();
                    TimerManager.Instance.TryGetTimer(rarityWeights[state].time, () =>
                    {
                         ChangeState(GetRandomRarity());
                    });
                    break;
                case AltarStateType.ZrFire:
                    StateMachine.SwitchState<ZrFireState>();
                    TimerManager.Instance.TryGetTimer(rarityWeights[state].time, () =>
                    {
                        ChangeState(GetRandomRarity());
                    });
                    break;
                case AltarStateType.Die:
                    StateMachine.SwitchState<AltarDieState>();
                    break;
            }
            currentState = state;
       
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
        public TrackEntry SetAnim(AnimationReferenceAsset _anim, bool _loops)
        {
            // 切换到指定动画，返回 TrackEntry 用于控制动画播放
            TrackEntry entry = animContorller.AnimationState.SetAnimation(0, _anim, _loops);
    
            entry.TrackTime = 0f;      // 重置动画到开头
            entry.MixDuration = 0f;    // 无过渡效果，立即切换
    
            return entry;
        }
        private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
        {
          
        }
        public void RemoveShill(AltarStateType  state)
        {
            rarityWeights.Remove(state);
            // 如果剩余数量为1，则切换为死亡状态
            if (rarityWeights.Count == 1)
            {
                Debug.LogError("Boss2死亡");
                ChangeState(AltarStateType.Die);
                return;
            }
            foreach (var skill in rarityWeights)
            {
                skill.Value.probability = 0.5f;
            }
        }
        //根据skillInfo信息随机获取技能
        public  AltarStateType GetRandomRarity()
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
                  
                        return kvp.Key;
                        
                }
            }
          return AltarStateType.Invincible;
        }
    }
}