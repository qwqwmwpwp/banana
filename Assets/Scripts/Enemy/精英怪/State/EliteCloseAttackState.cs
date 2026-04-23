using System.Collections;
using Enemy.精英怪.State;
using UnityEngine;


public class EliteCloseAttackState : EliteState
{
    private enum AttackType { Combo, Single }
    
    [Header("Combo Attack Settings")]
    public float comboPrepareTime = 0.8f;
    public float comboDashSpeed = 10f;
    public float comboAttackDuration = 1f;
    public float comboAttackDistance = 1f;
    public float comboMaxDashTime = 1f; // 新增：最大冲刺时间
    
    [Header("Single Attack Settings")]
    public float singleMoveSpeed = 3f; 
    public float singleAttackDuration = 0.5f;
    public float singleAttackDistance = 1f;
    public float singleMaxApproachTime = 2f; // 新增：最大接近时间
    
    private AttackType currentAttack;
    private Coroutine attackCoroutine;
    private bool isAttacking;
    private bool isAttackSequenceComplete;
    private Transform playerTransform = PlayerManager.instance.player.transform;
    public override void OnEnter()
    {
        currentAttack = Random.value <= 0.3f ? AttackType.Combo : AttackType.Single;
        isAttacking = false;
        isAttackSequenceComplete = false;
        attackCoroutine = null;
        
        eliteController.rb.velocity = Vector2.zero;
        Debug.Log($"Starting {currentAttack} attack at {Time.time}");
        
        // 立即开始攻击序列
        StartAttackSequence();
    }

    public override void OnUpdate()
    {
        if(eliteController==null) return;
        // 如果攻击序列已完成，检查是否应该切换状态
        if (isAttackSequenceComplete)
        {
            if (Mathf.Abs(playerTransform.transform.position.x - eliteController.transform.position.x) > 6f)
            {
                eliteController.ChangeState(EliteStateType.EliteChase);
            }
            return;
        }
        
        // 转向玩家
        eliteController.FacingDirection = playerTransform.transform.position.x > eliteController.transform.position.x ? 1 : -1;
        
        // 如果当前没有在执行攻击，且没有协程在运行，则开始攻击
        if (!isAttacking && attackCoroutine == null)
        {
            StartAttackSequence();
        }
    }

    private void StartAttackSequence()
    {
        if (attackCoroutine != null)
        {
            eliteController.StopCoroutine(attackCoroutine);
        }
        
        switch (currentAttack)
        {
            case AttackType.Combo:
                attackCoroutine = eliteController.StartCoroutine(ComboAttackSequence());
                break;
            case AttackType.Single:
                attackCoroutine = eliteController.StartCoroutine(SingleAttackSequence());
                break;
        }
    }

    private IEnumerator ComboAttackSequence()
    {
        isAttacking = true;
        
        // 准备阶段
        Debug.Log("Combo Attack - Preparing...");
        // TODO: 播放准备动画
        yield return new WaitForSeconds(comboPrepareTime);
        
        // 第一次冲刺攻击
        Debug.Log("Combo Attack - First Dash");
        
        yield return DashTowardPlayer(comboDashSpeed, comboAttackDistance, comboMaxDashTime);
        
        // 短暂停顿
        eliteController.rb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.2f);
        
        // 第二次攻击
        Debug.Log("Combo Attack - Second Strike");
        // TODO: 播放攻击动画
        yield return new WaitForSeconds(comboAttackDuration);
        
        CompleteAttackSequence();
    }

    private IEnumerator SingleAttackSequence()
    {
        isAttacking = true;
        
        // 接近玩家
        Debug.Log("Single Attack - Approaching");
        yield return DashTowardPlayer(singleMoveSpeed, singleAttackDistance, singleMaxApproachTime);
        
        // 执行攻击
        Debug.Log("Single Attack - Attacking");
      eliteController.rb.velocity = Vector2.zero;
        // TODO: 播放攻击动画
        yield return new WaitForSeconds(singleAttackDuration);
        
        CompleteAttackSequence();
    }

    private IEnumerator DashTowardPlayer(float speed, float targetDistance, float maxTime)
    {
        float startTime = Time.time;
        float initialDistance = Mathf.Abs(playerTransform.position.x - eliteController.transform.position.x);
        
        while (Mathf.Abs(playerTransform.position.x - eliteController.transform.position.x) > targetDistance)
        {
            // 检查是否超时
            if (Time.time - startTime >= maxTime)
            {
                Debug.LogWarning("Dash timed out!");
                break;
            }
            
            // 计算移动方向
            int direction = playerTransform.position.x > eliteController.transform.position.x ? 1 : -1;
            eliteController.rb.velocity = new Vector2(direction * speed, eliteController.rb.velocity.y);
            
            // 检查是否卡住（移动距离几乎没有变化）
            float currentDistance = Mathf.Abs(playerTransform.position.x - eliteController.transform.position.x);
            if (Mathf.Abs(currentDistance - initialDistance) < 0.1f * Time.deltaTime)
            {
                Debug.LogWarning("Enemy might be stuck!");
                break;
            }
            
            initialDistance = currentDistance;
            yield return null;
        }
    }

    private void CompleteAttackSequence()
    {
        eliteController.rb.velocity = Vector2.zero;
        isAttacking = false;
        isAttackSequenceComplete = true;
        attackCoroutine = null;
        Debug.Log("Attack sequence completed");
    }

    public override void OnExit()
    {
        if (attackCoroutine != null)
        {
            eliteController.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
        
        eliteController.rb.velocity = Vector2.zero;
        isAttacking = false;
        Debug.Log("Exiting EliteCloseAttackState");
    }
}