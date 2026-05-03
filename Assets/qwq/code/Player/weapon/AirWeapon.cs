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
    public Vector2 direction;//攻击方向
    public float moveSpeed = 3;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    public override bool IsEnter() => canAttack;

    private void Update()
    {
        if (ctx == null) return;

        if (ctx.detection.isPlatform || ctx.detection.isGrounded)
        {
            if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                return;
            }

            canAttack = true;
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

    }

    public override void OnEnter()
    {
        elementType = ctx.elementType;
        windupTimer = maxWindup;
        ctx.anim.SetAnim(ctx.anim.airAttackAnim, false);
        anim.SetTrigger("attack");
        direction = ctx.moveInput;
        if (ctx.moveInput.x < 0.1 && ctx.moveInput.x > -0.1)
            direction = new(ctx.transform.localScale.x, 0);
        

        boxCollider.enabled = true;
    }

    public override void OnExit()
    {
        boxCollider.enabled = false;
        cooldownTimer = maxCooldown;
    }

    public override void OnUpdate(float deltaTime)
    {

        if (windupTimer > 0)
        {
            ctx.rb.gravityScale = 0;
            ctx.velocity = new(direction.x*moveSpeed, 0);

            windupTimer -= deltaTime;
        }
        else
        {
            cooldownTimer = maxCooldown;
            canAttack = false;
        }
    }
}
