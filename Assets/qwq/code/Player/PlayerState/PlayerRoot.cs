using qwq;
using UnityEngine;

namespace HSM
{

    internal class PlayerRoot : State
    {
        private PlayerContext ctx;
        public readonly Act Act;
        public readonly Attack Attack;
        public readonly SwitchWeapon switchWeapon;
        public PlayerRoot(StateMachine m, PlayerContext ctx) : base(m, null)
        {
            this.ctx = ctx;
            Act = new Act(m, this, ctx);
            Attack = new Attack(m, this, ctx);
            switchWeapon = new SwitchWeapon(m, this, ctx);
        }
        protected override State GetInitialState() => Act;
        protected override State GetTransition() => null;
    }

    public class SwitchWeapon : State
    {
        private PlayerContext ctx;
        float t;
        float t_max = 1f;

        public SwitchWeapon(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;
        }

        protected override State GetInitialState()=>  null;
        protected override State GetTransition()
        {
            return t < 0 ? ((PlayerRoot)Parent).Act : null;
        }

        protected override void OnEnter()
        {
            ctx.anim.SetAnim(ctx.anim.switchAnim, true);
            t = t_max;

            if (ctx.elementType == ElementType.water)
                ctx.elementType = ElementType.ice;
            else
                ctx.elementType = ElementType.water;

            var rb = ctx.rb;
            if (rb != null)
            {
                ctx.velocity = new(0f, 0f);
                var v = rb.velocity;
                v.y = ctx.swichSpeed;
                rb.velocity = v;
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            t -= deltaTime;
        }
        protected override void OnExit()
        {
            ctx.isSwitchWeapon = false;
            ctx.swich_t = 0.5f;
        }
    }

    public class Attack : State
    {
        private PlayerContext ctx;
        public Attack(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;

        }
        protected override State GetTransition()
        {
            if (!ctx.activeWeapon.IsEnter())
            {
                return ((PlayerRoot)Parent).Act;
            }

            return null;
        }
        protected override void OnEnter()
        {
            Debug.Log("qwq");
            if (!ctx.detection.isGrounded && !ctx.detection.isPlatform)
                ctx.activeWeapon = ctx.airWeapon;
            else if (ctx.elementType == ElementType.ice)
                ctx.activeWeapon = ctx.iceWeapon;
            else
                ctx.activeWeapon = ctx.waterWeapon;

            ctx.activeWeapon.OnEnter();
        }
        protected override void OnUpdate(float deltaTime)
        {
            ctx.activeWeapon.OnUpdate(deltaTime);

        }
        protected override void OnExit()
        {
            ctx.activeWeapon.OnExit();

        }
    }

    public class Act : State
    {
        private PlayerContext ctx;

        public readonly Grounded Grounded;
        public readonly Airborne Airborne;
        public Act(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
        {
            this.ctx = ctx;
            Grounded = new Grounded(m, this, ctx);
            Airborne = new Airborne(m, this, ctx);
        }

        protected override State GetInitialState()
        {
            if (ctx.detection.isGrounded || ctx.detection.isPlatform) return Grounded;
            else return Airborne;
        }
        protected override State GetTransition()
        {
            if (ctx.isSwitchWeapon && ctx.swich_t <= 0) return ((PlayerRoot)Parent).switchWeapon;

            if (ctx.isAttackInput&&(!ctx.activeWeapon||ctx.activeWeapon.IsEnter())) return ((PlayerRoot)Parent).Attack;

            return null;
        }
    }
}


    