using System.Collections;
using System.Collections.Generic;
using Enemy.精英怪.State;
using UnityEngine;


public class EliteRemoteAttackState : EliteState
{
    [Header("蓄力時間")]
    public float chargeTime = 1.5f;
    [Header("火球数量")]
    public int fireballsCount = 3;
    [Header("火球间隔")]
    public float fireballInterval = 0.3f;
    [Header("火球速度")]
    public float fireballSpeed = 10f;
    private Coroutine attackCoroutine;
    
    public override void OnEnter()
    {
       // 播放动画
        attackCoroutine = eliteController.StartCoroutine(RangedAttackSequence());
    }

 
    private IEnumerator RangedAttackSequence()
    {
        // 蓄力阶段
        yield return new WaitForSeconds(chargeTime);

        // 发射火球阶段
        for (int i = 0; i < fireballsCount; i++)
        {
            eliteController.FireFireball();
            if (i < fireballsCount - 1)
            {
                yield return new WaitForSeconds(fireballInterval);
            }
        }
        
        Debug.Log("攻击结束");
        // 攻击完成后进入冷却并返回巡逻状态
        eliteController.StartRangedAttackCooldown();
      eliteController.ChangeState(EliteStateType.EliteChase); 
    }

    public override void OnExit()
    {
        attackCoroutine = null;
    }
}
