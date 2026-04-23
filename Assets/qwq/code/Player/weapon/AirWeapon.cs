using qwq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWeapon : Weapon
{
    Animator anim;
    BoxCollider2D boxCollider;

 public ElementEnmu elementEnmu;

    public bool isAttack;
    float t;
    public float t_max = 0.15f;
   float cooling;
    public float Cooling_max = 0.1f;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override bool IsEnter() => isAttack;

    private void Update()
    {
        if (ctx == null) return;
        
        elementEnmu = ctx.elementEnmu;

        if (ctx.detection.isPlatform || ctx.detection.isGrounded)
        {
            if (cooling > 0)
            {
                cooling -= Time.deltaTime;
            }

            isAttack = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<IInteraction>()?.Trigger(gameObject);
        Debug.Log(collision.name);
    }

    public override void OnEnter()
    {
        t = t_max;
        ctx.anim.SetAnim(ctx.anim.airAttackAnim, false);
        anim.SetTrigger("attack");

        boxCollider.enabled = true;
    }

    public override void OnExit()
    {
        boxCollider.enabled = false;

    }

    public override void OnUpdate(float deltaTime)
    {

        if (t > 0)
        {
            ctx.velocity = Vector2.zero;
            ctx.rb.gravityScale = 0;
            ctx.rb.velocity = new(ctx.rb.velocity.x, 0);
            t -= deltaTime;
        }
        else
        {
            cooling = Cooling_max;
            isAttack = false;
        }
    }
}
