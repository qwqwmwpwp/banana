using qwq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirWeapon : Weapon
{
    Animator anim;
    BoxCollider2D boxCollider;

    public ElementType elementType;  // 元素类型

    public bool canAttack;  // 攻击许可
    float windupTimer;  // 前摇计时
    public float maxWindup = 0.15f;  // 最大前摇
    float cooldownTimer;  // 冷却计时
    public float maxCooldown = 0.1f;  // 最大冷却
    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override bool IsEnter() => canAttack;

    private void Update()
    {
        if (ctx == null) return;
        
        elementType = ctx.elementType;

        if (ctx.detection.isPlatform || ctx.detection.isGrounded)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
            }

            canAttack = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public override void OnEnter()
    {
        windupTimer = maxWindup;
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

        if (windupTimer > 0)
        {
            ctx.velocity = Vector2.zero;
            ctx.rb.gravityScale = 0;
            ctx.rb.velocity = new(ctx.rb.velocity.x, 0);
            windupTimer -= deltaTime;
        }
        else
        {
            cooldownTimer = maxCooldown;
            canAttack = false;
        }
    }
}
