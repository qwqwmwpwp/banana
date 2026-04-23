using PhysicsExtension;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace qwq
{
    public class PlayerDetection : MonoBehaviour
    {
        #region === 地面检测与输入信息 ===
        [Header("检测信息")]
        [SerializeField] protected Transform groundCheck;            // 地面检测点
        [SerializeField] protected Vector2 groundSize = Vector2.one; // 检测半径
        [SerializeField] protected LayerMask groundLayer;           // 地面层
        [SerializeField] protected float slopeCheckDistance = 0.5f;  // 斜坡检测距离
        [SerializeField] protected float coyoteTime = 0.08f;         // “土狼时间”，防止落地抖动

        [SerializeField] protected LayerMask platformLayer;           // 平台层
        #endregion
        #region === 状态变量 ===
        public bool isGrounded; //{ get; private set; }     // 当前是否接触地面
        public bool isPlatform;
        private float groundCoyoteTimer;                       // 土狼时间计时器
        private float platformCoyoteTimer;                       // 土狼时间计时器

        #endregion
         public Vector2 center;

        private void Update()
        {
            UpdateGroundCheck();
            UpdatePlatformCheck();
        }

        private void UpdatePlatformCheck()
        {
            
            bool hit = Physics2D.OverlapBox(transform.position, groundSize,0f, platformLayer);

            if (hit)
            {
                isPlatform = true;
                platformCoyoteTimer = coyoteTime;
            }
            else if(platformCoyoteTimer>0)
            {
                // 未检测到 → 递减土狼计时器
                platformCoyoteTimer -= Time.deltaTime;

            }
                // 只要土狼时间没耗尽，仍视为在地面上（防止落地瞬间切回空中状态）
                isPlatform = platformCoyoteTimer > 0f;

        }

        private void UpdateGroundCheck()
        {
            // 使用 OverlapCircle 检测地面（比 Raycast 更稳定）
            bool hit = Physics2D.OverlapBox(transform.position, groundSize,0f, groundLayer);

            if (hit)
            {
                // 检测到地面 → 立即重置土狼计时
                isGrounded = true;
                groundCoyoteTimer = coyoteTime;
            }
            else
            {
                // 未检测到 → 递减土狼计时器
                groundCoyoteTimer -= Time.deltaTime;

                // 只要土狼时间没耗尽，仍视为在地面上（防止落地瞬间切回空中状态）
                isGrounded = groundCoyoteTimer > 0f;
            }

        }
        

        void OnDrawGizmos()
        {
            Handles.DrawWireCube(transform.position,  groundSize);
            // 画一个小球表示圆心

            //Gizmos.DrawSphere(center, 0.1f);
            // 画一个圆环
            Gizmos.color = Color.blue;
            Handles.DrawWireDisc(center+(Vector2)transform.position, Vector3.forward,0.1f);
        }
    }


}