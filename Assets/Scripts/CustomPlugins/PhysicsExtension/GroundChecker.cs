using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhysicsExtension
{
    public class GroundChecker : MonoBehaviour
    {
        // 通用 Layer 層級
        protected int groundLayer = 1 << 7;             // 地板層級 (不能穿越)
        protected int platformLayer = 1 << 8;           // 平台層級 (可以穿越)

        // 自身組件 
        protected Rigidbody2D rb;                       
        protected Collider2D bodyCollider;

        // 地面檢測參數及狀態
        [Header("地面檢測")]
        public bool isGrounded;                         // 觸地

        [Tooltip("地面檢測範圍")][SerializeField]
        protected float groundCheckDistance = 1.0f;     // 從碰撞體最底部開始算起

        // 斜坡檢測參數及狀態
        [Header("斜坡檢測")]
        public bool isSloped;                           // 斜坡
        protected float slopeDownAngle;                 // 下方斜坡度數
        protected float slopeDownAngleOld;              // 前一幀下方斜坡度數
        public Vector2 slopeNormalPerp;              // 斜坡切線方向             

        [Tooltip("坡度檢測範圍")][SerializeField]
        protected float slopeCheckDistance = 1.0f;      // 從碰撞體最底部開始算起

        private void OnEnable()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) GetComponentInChildren<Rigidbody2D>();

            bodyCollider = GetComponent<Collider2D>();
            if (bodyCollider == null) GetComponentInChildren<Collider2D>(); 
        }

        private void FixedUpdate()
        {
            CheckSloped();

            CheckGrounded();
        }

        // 觸地檢測 有需要用到的角色 在具體角色中搭載此物件再搭配 isGrounded 就可以做到
        protected virtual void CheckGrounded()
        {
            Bounds bounds = bodyCollider.bounds;

            Vector2 center = new Vector2(bounds.center.x, bounds.min.y);

            RaycastHit2D hitPlatform = Physics2D.Raycast(center, Vector2.down, groundCheckDistance, platformLayer);
            RaycastHit2D hitGround = Physics2D.Raycast(center, Vector2.down, groundCheckDistance, groundLayer);

            isGrounded = (Mathf.Abs(rb.velocity.y) <= 0.01f) && (hitPlatform || hitGround);
            if (!isGrounded) isGrounded = isSloped && (hitPlatform || hitGround);
        }

        // 斜坡檢測 有需要用到的角色 在具體角色中搭載此物件再搭配 isSloped 就可以做到
        protected virtual void CheckSloped()
        {
            // 沒有剛體和碰撞盒不會生效
            if (rb == null || bodyCollider == null) return;

            Bounds bounds = bodyCollider.bounds;
            Vector2 checkPos = new Vector2(bounds.center.x, bounds.min.y);

            CheckVerticalSlope(checkPos);
            CheckHorizontalSlope(checkPos);
        }

        // 垂直斜坡檢測
        protected virtual void CheckVerticalSlope(Vector2 checkPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);
            if (!hit) hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, platformLayer);

            if (hit)
            {
                slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (slopeDownAngleOld != slopeDownAngle)
                {
                    isSloped = true;
                }
                slopeDownAngleOld = slopeDownAngle;

#if UNITY_EDITOR
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
                Debug.DrawRay(hit.point, hit.normal, Color.green);
#endif
            }


        }   

        // 水平斜坡檢測
        protected virtual void CheckHorizontalSlope(Vector2 checkPos)
        {
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
            if (!slopeHitFront) slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, platformLayer);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);
            if (!slopeHitBack) slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, platformLayer);

#if UNITY_EDITOR
        Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.blue);
        Debug.DrawRay(checkPos, -transform.right * slopeCheckDistance, Color.blue);
#endif

            if (slopeHitFront)
            {
                isSloped = true;
            }
            else if(slopeHitBack)
            {
                isSloped = true;
            }
            else
            {
                isSloped = false;
            }
        }

    }
}