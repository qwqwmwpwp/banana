using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using PhysicsExtension;

namespace EnemyCommon
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D), typeof(GroundChecker))]
    public class EnemyCommonController : EnemyBase, IStateMachineOwner
    {
        [HideInInspector] public int playerLayer = 1 << 6;
        
        protected StateMachine stateMachine = new StateMachine();
        [Header("當前狀態")]
        public EnemyCommonStateType currentState;

        [Header("組件")]
        public GroundChecker groundChecker;
        public Collider2D bodyCollider;

        [Header("移動參數")]
        [SerializeField] public float moveSpeed;                    // 移動速度
        [Tooltip("每次到達巡邏點後停留時間")]
        [SerializeField] public float stayDuration;                      
        [Tooltip("巡邏點")]
        public List<Transform> patrolPoints;
        [HideInInspector] public float stayTimer;                   // 停留時間計時器

        [Header("索敵參數")]
        [Tooltip("索敵距離")]
        public float detectionRadius = 5f;
        [Tooltip("索敵角度")]
        public float detectionAngle = 180f;
        [Tooltip("索敵最小位置")]
        public Transform detectionPosMin;
        [Tooltip("索敵最大位置")]
        public Transform detectionPosMax;

        [Header("重生點")]
        public Transform respawnPoint;

        public override void Init(EnemySpawnContext ctx)
        {
            respawnPoint = ctx.spawnPoint;
            patrolPoints = ctx.patrolPoints;
            detectionPosMin = patrolPoints[0];
            detectionPosMax = patrolPoints[patrolPoints.Count - 1];
        }
    }
}