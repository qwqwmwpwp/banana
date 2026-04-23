using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingState_BlackRedFire_Ranged : BlackRedFire_Ranged_State
{
    private float timer;
    public override void OnEnter()
    {
        base.OnEnter();
        timer = enemy.ChargeTime;
    }

    public override void OnExit()
    {
        base.OnExit();

        //Ë¢ÐÂ¹¥»÷ÀäÈ´
        enemy.fireballCooldownTimer = enemy.fireballCooldown;

    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        timer -= Time.deltaTime;

        if (timer < 0 && enemy!=null)
        {
            enemy.InstantiateFireball();

            enemy.ChangeState(EnemyStateType.Idle);

        }
    }
}
