using System;
using System.Collections.Generic;
using Enemy.Boss.TailKing_Boss_3_.State;
using Enemy.Boss.TailKing_Boss_3_.Status;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace Enemy.Boss.TailKing_Boss_3_
{
    public class TailKingController : EnemyBase,IStateMachineOwner
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
        public float current_speed;
        public float normalSpeed;
        public float fasterSpeed;
        [Header("技能设置")]
        public SkillInfo NormalChop;
        public SkillInfo TripleChop;
        private float now_speed;
        [Header("后退")]
        public float wait_time;
        [Header("朝向")] 
        public int Direction = 1;  // 1:向右 -1:向左
        
        
         public Transform characterPosition;
         [HideInInspector]
         public Transform player;
         public Transform character;
         [Header("攻击位置")]
         public Transform attackPosition1;
         public float attackRange1;
         private float attackDis1;
         public Transform attackPosition2;
         public float attackRange2;
         private float attackDis2;
         
         
        [HideInInspector] 
        public Color originalColor;
        private StateMachine StateMachine = new StateMachine();
        private Vector2 scale;
        private int playerLayer = 1 << 6;
       
        public Dictionary<TailKingType, SkillInfo> skillInfo = new Dictionary<TailKingType, SkillInfo>();
        [HideInInspector]  public TailKingStatus TailKingstatus;
        [SerializeField] private TailKingType currentState;
        public TailKingType CurrentState
        {
            get
            {
                return currentState;
            }
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
            player = PlayerManager.instance.player.transform;
            StateMachine.Init(this);
            scale = character.transform.localScale;
            // 记录受击前的颜色
            originalColor = animContorller.skeleton.GetColor();
            rb = GetComponent<Rigidbody2D>();
            TailKingstatus = GetComponent<TailKingStatus>();
            skillInfo.Add(TailKingType.NormalChop, NormalChop);
            skillInfo.Add(TailKingType.TripleChop, TripleChop);
            ChangeState(TailKingType.ChaseState);
            current_speed = normalSpeed;
            attackDis1 = attackPosition1.position.x - character.transform.position.x;
            attackDis2 = attackPosition2.position.x - character.transform.position.x;
        }
        
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
               TailKingstatus.SetShield(false);
            }

            if (player.transform.position.x > characterPosition.transform.position.x)
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
            // 判断事件名字
            if (e.Data.Name == "attack")
            {
                // 事件来源的动画名
                string animName = trackEntry.Animation.Name;
                AudioManager.instance.PlaySFX(36,characterPosition.position);
                switch (currentState)
                {
                   case TailKingType.NormalChop:
                        AttackTrigger(attackPosition1,attackRange1);
                       break;
                   case TailKingType.TripleChop:
                       AttackTrigger(attackPosition2,attackRange2);
                        break;
                }
             
            }
        }
        // 攻击判定
        public void AttackTrigger(Transform attack,float range)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(attack.position, range,playerLayer);

            foreach (var hit in colliders)
            {
                PlayerStats stats = hit.GetComponent<PlayerStats>();
                if (stats != null)
                {
                    TailKingstatus.DoDamage(stats, AttackType.Melee, 1f);
                }
            }
        }
        public void ChangeState(TailKingType state)
        {
             currentState = state;
            switch (state)
            {
                case TailKingType.Idle:
                    StateMachine.SwitchState<IdleState>();
                    break;
                 case TailKingType.ChaseState:
                     StateMachine.SwitchState<ChaseState>();
                     break;
                 case TailKingType.NormalChop:
                     StateMachine.SwitchState<NormalChop>();
                     break;
                 case TailKingType.TripleChop:
                     StateMachine.SwitchState<TripleChop>();
                     break;
                 case TailKingType.Die:
                     StateMachine.SwitchState<DieState>();
                     break;
                 case TailKingType.BackState:
                     StateMachine.SwitchState<BackState>();
                     break;
            }
        }
        public void Filp()
        {
            character.localScale = new Vector3(scale.x * Direction, scale.y);
            attackPosition1.position = new Vector3(character.position.x + Direction * attackDis1, attackPosition1.position.y);
           attackPosition2.position = new Vector3(character.position.x + Direction * attackDis2, attackPosition2.position.y);
        }

        private void OnDrawGizmos()
        {
            // 绘制第一个攻击范围
            if (attackPosition1 != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(attackPosition1.position, attackRange1);
            }

            // 绘制第二个攻击范围
            if (attackPosition2 != null)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(attackPosition2.position, attackRange2);
            }
        }
        void OnDestroy()
        {
            // 在这里清理资源、取消订阅事件、停止协程等
            StateMachine?.Clear();
            StopAllCoroutines();
        }
    }
}