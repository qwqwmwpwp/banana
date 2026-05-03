using qwq;
using UnityEngine;

namespace HSM
{
    //空中
    public class Airborne : State
    {
        readonly PlayerContext ctx;
        public readonly Falling falling;
        public readonly DoubleJump doubleJump;

        public Airborne(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;
            falling = new Falling(m, this, ctx);
            doubleJump = new DoubleJump(m, this, ctx);

            Add(new DelayActivationActivity { seconds = 0.1f });
        }

        protected override State GetInitialState()
        {
            return falling;
        }
        protected override State GetTransition()
        {
            return ctx.detection.isGrounded||ctx.detection.isPlatform ? ((Act)Parent).Grounded : null;
        }
        protected override void OnUpdate(float deltaTime)
        {
        }
        protected override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.jumpAnim, true);
        }
    }

    public class Falling : State
    {
        readonly PlayerContext ctx;

        public Falling(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }
        protected override State GetTransition()
        {
            if (ctx.jumpPressed && ctx.canDoubleJump)
            {
                ctx.jumpPressed = false;
                ctx.canDoubleJump = false;

                var rb = ctx.rb;
                if (rb != null)
                {
                    var v = rb.velocity;
                    v.y = ctx.jumpSpeed;
                    rb.velocity = v;
                }

                return ((Airborne)Parent).doubleJump;
            }
            return null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            var target = ctx.moveInput.x * ctx.moveSpeed;
            ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, target, ctx.acceleration * deltaTime);

            float localScale_x;
            if (ctx.moveInput.x > 0.1f)
            {
                localScale_x = Mathf.Abs(ctx.transform.localScale.x);
            }
            else if (ctx.moveInput.x < -0.1f)
            {
                localScale_x = -1 * Mathf.Abs(ctx.transform.localScale.x);
            }
            else
            {
                localScale_x = ctx.transform.localScale.x;
            }
            ctx.transform.localScale = new(localScale_x, 1, 1);
        }
    }

    public class DoubleJump : State
    {
        readonly PlayerContext ctx;
        float t = 0;
        public DoubleJump(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;

        }
        protected override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.jumpAnim, true);
            t = ctx.jumpInertiaTime;
        }
        protected override void OnUpdate(float deltaTime)
        {
            t -= deltaTime;
        }
        protected override State GetTransition()
        {
            return t < 0f ? ((Airborne)Parent).falling : null;
        }
    }
}
