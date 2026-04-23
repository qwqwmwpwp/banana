using System.Collections;

using UnityEngine;
namespace Enemy.Boss
{

    public class MagicCircleController : MonoBehaviour
{
    [Header("法阵设置")]
    public float moveSpeed = 3f;     // 法阵移动速度
    public float preparationTime = 2f;   // 准备时间
    public float delayAfterPreparation = 1f; // 准备结束后的延迟
    public int damagePerHit = 10;        // 每次伤害值
    public int numberOfHits = 5;         // 伤害次数
    public float timeBetweenHits = 1f;   // 伤害间隔

   
    private Transform player;             
    private bool isActive = false;        // 法阵是否激活
  

    private Animator animator;
    //检测
    [SerializeField]Vector2 direction = Vector2.up;
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float angle;
    [SerializeField] private float distance;
    [SerializeField] private LayerMask layerMask;
    void Start()
    {
        // 查找玩家
       animator = GetComponent<Animator>();
        player = PlayerManager.instance.player.transform;
        StartCoroutine(ActivateMagicCircle());
    }

    void Update()
    {
        // 在准备阶段跟随玩家X坐标
        if (isActive)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector2(player.position.x, transform.position.y),
                moveSpeed * Time.deltaTime
            );
            
        }
    }

    IEnumerator ActivateMagicCircle()
    {
        isActive = true;
        
        // 准备阶段
        float timer = 0f;
        while (timer < preparationTime)
        {
            timer += Time.deltaTime;
            // 可以在这里添加法阵准备阶段的视觉效果
            yield return null;
        }
        
        // 准备结束，停止跟随
        isActive = false;
        
        // 延迟
        yield return new WaitForSeconds(delayAfterPreparation);
        animator.SetBool("isAttack",true);
        
        // 造成伤害
        for (int i = 0; i < numberOfHits; i++)
        {
            // 检查玩家是否仍在法阵上方
            if (IsPlayerAboveCircle(out PlayerStats playerStats))
            {
                // 对玩家造成伤害
                playerStats.TakeDamage(damagePerHit,AttackType.Melee);
                
             
            }
            
            // 等待下一次伤害
            yield return new WaitForSeconds(timeBetweenHits);
        }
        
        // 伤害结束后销毁法阵
        Destroy(gameObject);
    }

    bool IsPlayerAboveCircle(out PlayerStats playerStats)
    {
        if (player == null) {playerStats = null; return false;}
       
        // 执行矩形射线检测
        RaycastHit2D hit = Physics2D.BoxCast(
            transform.position, 
            boxSize, 
            angle, 
            direction, 
            distance, 
            layerMask
        );
        if(hit.collider==null||hit.collider.tag!="Player") { playerStats = null;return false;}
        playerStats = hit.collider.GetComponent<PlayerStats>();
        return hit.collider!=null;
    }
  

    void OnDrawGizmos()
    {
        // 绘制检测区域
        Gizmos.color = Color.green;
        
       
        Gizmos.DrawWireCube(transform.position + (Vector3)direction * distance, new Vector3(boxSize.x, boxSize.y, 0));
    }
}


}