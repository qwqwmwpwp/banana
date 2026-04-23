using qwq;
using UnityEngine;

namespace HSM
{
    //地面
    public class Grounded : State
    {
        readonly PlayerContext ctx;
        public readonly Idle Idle;
        public readonly Move Move;

        public Grounded(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;
            Idle = new Idle(m, this, ctx);
            Move = new Move(m, this, ctx);
            Add(new DelayActivationActivity { seconds = 0.1f });
        }

        protected override State GetInitialState() => Idle;

        protected override State GetTransition()
        {


            if (ctx.detection.isGrounded||ctx.detection.isPlatform)
            {
                if (ctx.jumpPressed)
                {
                    ctx.jumpPressed = false;
                    var rb = ctx.rb;

                    if (rb != null)
                    {
                        var v = rb.velocity;
                        v.y = ctx.jumpSpeed;
                        rb.velocity = v;
                    }

                    return ((Act)Parent).Airborne;
                }
                return null;
            }
            return ((Act)Parent).Airborne;
        }
    }

    public class Idle : State
    {
        readonly PlayerContext ctx;

        public Idle(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }
        protected override State GetTransition()
        {
            return Mathf.Abs(ctx.move.x) > 0.01f ? ((Grounded)Parent).Move : null;
        }

        protected override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.idleAnim, true).MixDuration = 0.05f;
        }

        protected override void OnUpdate(float deltaTime)
        {
            ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, 0, ctx.accel * deltaTime);
        }
    }

    public class Move : State
    {
        readonly PlayerContext ctx;

        public Move(StateMachine machine, State parent, PlayerContext ctx) : base(machine, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetTransition()
        {
            return Mathf.Abs(ctx.move.x) <= 0.1f ? ((Grounded)Parent).Idle : null;
        }

        protected override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.moveAnim, true).MixDuration = 0.05f;
        }

        protected override void OnUpdate(float deltaTime)
        {
            var target = ctx.move.x * ctx.moveSpeed;
            ctx.velocity.x = Mathf.MoveTowards(ctx.velocity.x, target, ctx.accel * deltaTime);

            float localScale_x;
            if (ctx.move.x > 0.1f)
            {
                localScale_x = Mathf.Abs(ctx.transform.localScale.x);
            }
            else if (ctx.move.x < -0.1f)
            {
                localScale_x = -1 * Mathf.Abs(ctx.transform.localScale.x);
            }
            else
            {
                localScale_x = ctx.transform.localScale.x;
            }
            if (ctx.transform.localScale.x != localScale_x)
            {
                ctx.anim.SetAnim(ctx.anim.idleAnim, true).MixDuration = 0.05f;
                ctx.transform.localScale = new(localScale_x, 1, 1);
                ctx.anim.SetAnim(ctx.anim.moveAnim, true).MixDuration = 0.2f;
            }
        }

    }

}