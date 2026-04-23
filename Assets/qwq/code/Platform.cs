using HSM;
using qwq;
using UnityEngine;
namespace qwq
{
    public class Platform : MonoBehaviour, IInteraction
    {
        private PlatformState root;
        HSM.StateMachine machine;

        public ElementEnmu elementEnmu;
        public SpriteRenderer sprite;
        public BoxCollider2D collider2d;

        public float inertialCoefficient = 0f;
        public float InitialState_t = 3f;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            collider2d = GetComponent<BoxCollider2D>();


            root = new PlatformState(machine, this);
            StateMachineBuilder builder = new StateMachineBuilder(root);
            machine = builder.Build();


        }

        private void Update()
        {
            machine.Tick(Time.deltaTime);
        }

        public void Trigger(GameObject gObj)
        {
            ElementEnmu newElementEnmu = elementEnmu;

            if (gObj.GetComponent<Bullet>() is Bullet bullet)
            {
                if (bullet.enmu != ElementEnmu.water && bullet.enmu != ElementEnmu.ice)
                    return;

                newElementEnmu = bullet.enmu;
                //Destroy(bullet.gameObject);
            }

            if (gObj.GetComponent<AirWeapon>() is AirWeapon airWeapon)
            {
                if (airWeapon.elementEnmu != ElementEnmu.water && airWeapon.elementEnmu != ElementEnmu.ice)
                    return;

                newElementEnmu = airWeapon.elementEnmu;
            }

            elementEnmu = newElementEnmu;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            root.TriggerEnter(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            root.TriggerExit(collision);
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
            root.CollisionEnter(collision);
        }

        public void OnCollisionExit2D(Collision2D collision)
        {
            root.CollisionExit(collision);
        }

    }
}

namespace HSM
{
    public class IcePlatform : InteractionState
    {
        Platform Platform;
        float inertia = 0f;
        Player player;
        public IcePlatform(StateMachine machine, State parent, Platform platform) : base(machine, parent)
        {
            Platform = platform;
        }

        protected override void OnEnter()
        {
            Platform.sprite.color = Color.white;
            Platform.collider2d.isTrigger = false;
            //Platform.collider2d.usedByEffector = true;
            player = null;
        }

        protected override void OnCollisionEnter(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Player>() is Player player)
            {
                this.player = player;
                inertia = player.ctx.velocity.x;
            }
        }

        protected override void OnCollisionExit(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Player>())
            {
                player = null;
            }
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (player)
            {
                if (Mathf.Abs(player.ctx.rb.velocity.x) < Mathf.Abs(inertia))
                {
                    player.ctx.velocity.x = Mathf.MoveTowards(player.ctx.velocity.x,
                        inertia,
                        deltaTime * player.ctx.accel * Platform.inertialCoefficient);
                }
                inertia = player.ctx.velocity.x;
            }
        }
    }

    public class WaterPlatform : InteractionState
    {
        Platform Platform;
        Player player;
        Vector2 direction;


        public WaterPlatform(StateMachine machine, State parent, Platform platform) : base(machine, parent)
        {
            Platform = platform;
        }

        protected override void OnEnter()
        {
            Platform.sprite.color = Color.blue;
            Platform.collider2d.isTrigger = true;
            //Platform.collider2d.usedByEffector = false;
            player = null;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (player)
            {
                player.ctx.velocity.x = direction.x * 15;
                player.ctx.rb.velocity =new(0, direction.y * 15);

            }
        }

        protected override void OnTriggerEnter(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Player>() is Player player)
            {
                this.player = player;

                Vector2 vector = player.ctx.rb.velocity;
                float angle = Mathf.Atan2(vector.y, vector.x) * Mathf.Rad2Deg;
                Quaternion rotation = Platform.transform.rotation;

                // 将Vector2转为Vector3，旋转后再转回Vector2
                Vector3 vec3 = new Vector3(0, 1, 0);
                direction = (rotation * vec3).normalized;
                float diff = (Mathf.Abs(Platform.transform.eulerAngles.z + 90 - angle) % 360);
                diff = diff > 180 ? 360 - diff : diff;
                if (diff >= 90)
                {
                    direction *= -1;
                }
            }
        }

        protected override void OnTriggerExit(Collider2D collision)
        {
            if (collision.gameObject.GetComponent<Player>())
                player = null;
        }
    }

    public class PlatformState : InteractionState
    {
        WaterPlatform water;
        IcePlatform ice;
        Platform Platform;

        public ElementEnmu initialElement;
        public float initialState_t;
        public PlatformState(StateMachine machine, Platform platform) : base(machine, null)
        {
            water = new WaterPlatform(machine, this, platform);
            ice = new IcePlatform(machine, this, platform);
            Platform = platform;
            initialElement = Platform.elementEnmu;
            initialState_t = platform.InitialState_t;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (Platform.elementEnmu != initialElement && initialState_t > 0) initialState_t -= deltaTime;
        }

        protected override State GetInitialState()
        {

            switch (Platform.elementEnmu)
            {
                case ElementEnmu.water:
                    return water;
                case ElementEnmu.ice:
                    return ice;
            }
            return null;
        }

        protected override State GetTransition()
        {
            if (initialState_t <= 0) Platform.elementEnmu = initialElement;

            InteractionState newState = null;

            switch (Platform.elementEnmu)
            {
                case ElementEnmu.water:
                    newState = water;
                    break;
                case ElementEnmu.ice:
                    newState = ice;
                    break;
            }


            if (newState != ActiveChild)
            {
                initialState_t = Platform.InitialState_t;
                return newState;
            }

            return null;
        }
    }
}