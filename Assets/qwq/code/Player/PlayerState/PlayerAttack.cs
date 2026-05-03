using qwq;

namespace HSM
{
    public class PlayerAttack : State
    {
        private PlayerContext ctx;
        public PlayerAttack(StateMachine m, State parent, PlayerContext ctx) : base(m, parent)
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
}


    