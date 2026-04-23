using UnityEngine;

public class ChaseState_BlackWhiteFire : BlackWhiteFire_State
{
    PlayerController player;

    public override void OnEnter()
    {
        base.OnEnter();
        player = PlayerManager.instance.player;
    }

    public override void OnExit()
    {
        base.OnExit();
    }



    public override void OnUpdate()
    {
        base.OnUpdate();
        if (enemy == null)
        {
            return;
        }

        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position, player.transform.position, enemy.chaseSpeed * Time.deltaTime);

        if (Vector3.Distance(player.transform.position, enemy.transform.position) < enemy.SelfDestructingRadius * 0.75f)
        {
            enemy.rb.velocity = new Vector2(0, 0);
            enemy.StartSelfDestructing();
        }
    }

}
