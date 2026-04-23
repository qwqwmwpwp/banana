using System;
using Unity.VisualScripting;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    [Header("参数")]
    public float rotationSpeed;
    [Header("伤害")]
    public int damage;
    [Header("移动速度")] 
    public float moveSpeed;
    [Header("目标方向")]
    public Vector3 direction;
    [Header("攻击设置")]
    public float attackRange = 2f;

    public LayerMask playerLayer;
    
    [Header("攻击偏移")]
    public Vector2 attackOffset = Vector2.zero;
    
    private  Transform target;
    private Rigidbody2D rb;
    private float rotateSpeed = 100f;
   
    private  float rotationSmoothness = 2f;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        target = PlayerManager.instance.player.transform;
        direction = (target.position - transform.position).normalized;
    }
  
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("进入触发器");
        if (other.CompareTag("Player"))
        {
            Debug.Log("玩家进入检测区域");
            PlayerStats stats = other.GetComponent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage, AttackType.Melee);
            }
        }
    }
    private void Update()
    {
        
        if (CheckPlayer())
        {
            direction = (target.position - transform.position).normalized; 
            float angle = GetAngle(direction, -transform.up);
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            //rb.velocity = -transform.up * moveSpeed;
        }
      
    }
    bool CheckPlayer()
    {
        // 使用 Physics2D.OverlapCircle 检测范围内的玩家
        Vector2 attackPosition = (Vector2)transform.position + attackOffset;
        Collider2D playerCollider = Physics2D.OverlapCircle(attackPosition, attackRange, playerLayer);

        return playerCollider != null;

    }
    /// <summary>
    /// 判断两个向量是否在同一方向
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <param name="threshold"></param>
    /// <returns></returns>
    public bool AreVectorsSameDirection(Vector3 vec1, Vector3 vec2, float threshold = 0.95f)
    {
        if (vec1.sqrMagnitude < 0.001f || vec2.sqrMagnitude < 0.001f)
            return false; // 避免零向量
    
        float dot = Vector2.Dot(vec1.normalized, vec2.normalized);
        Debug.Log(dot);
        return dot >= threshold;
    }
    /// <summary>
    /// 计算两向量之间夹角
    /// </summary>
    /// <param name="vec1"></param>
    /// <param name="vec2"></param>
    /// <returns></returns>
   public float GetAngle(Vector3 vec1, Vector3 vec2)
    {
        float dot = Vector3.Dot(vec1, vec2);
        float angle = Mathf.Acos(dot / (vec1.magnitude * vec2.magnitude));
        return angle;
    }

    // void SmoothRotateSlerp(Vector3 currentDirection, Vector3 targetDirection)
    // {
    //     if (targetDirection.sqrMagnitude < 0.001f) return;
    //
    //     Quaternion targetRotation = Quaternion. FromToRotation(currentDirection, targetDirection);
    //     Quaternion newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSmoothness);
    //     transform.rotation = newRotation; 
    // }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector2 attackPosition = (Vector2)transform.position + attackOffset;
        Gizmos.DrawWireSphere(attackPosition, attackRange);
    }
    
}