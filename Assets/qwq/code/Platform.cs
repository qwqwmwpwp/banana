using HSM;
using qwq;
using UnityEditor.UIElements;
using UnityEngine;
namespace qwq
{
    public class Platform : MonoBehaviour, IInteraction
    {
        private PlatformRoot root;
        HSM.StateMachine machine;

        public ElementType elementEnmu;
        [HideInInspector] public SpriteRenderer sprite;
        [HideInInspector] public BoxCollider2D collider2d;
        public float playerSpeed = 5;

      public int layerIce = -1;
       public int layerWater = -1;

        public float inertialCoefficient = 0f;
        public float InitialState_t = 3f;

        private void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            collider2d = GetComponent<BoxCollider2D>();


            root = new PlatformRoot(machine, this);
            StateMachineBuilder builder = new StateMachineBuilder(root);
            machine = builder.Build();

            layerIce = 8;
            layerWater = 10;
        }

        private void Update()
        {
            machine.Tick(Time.deltaTime);
        }

        public void Trigger(GameObject gObj)
        {
            ElementType newElementEnmu = elementEnmu;

            if (gObj.GetComponent<Bullet>() is Bullet bullet)
            {
                if (bullet.enmu != ElementType.water && bullet.enmu != ElementType.ice)
                    return;

                newElementEnmu = bullet.enmu;
            }

            if (gObj.GetComponent<AirWeapon>() is AirWeapon airWeapon)
            {
                if (airWeapon.elementType != ElementType.water && airWeapon.elementType != ElementType.ice)
                    return;

                newElementEnmu = airWeapon.elementType;
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
    //子状态
    public class PlatformRoot : InteractionState
    {
        public WaterPlatform water;
        public IcePlatform ice;
        Platform Platform;

        public ElementType initialElement;
        public float initialState_t;
        public PlatformRoot(StateMachine machine, Platform platform) : base(machine, null)
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
                case ElementType.water:
                    return water;
                case ElementType.ice:
                    return ice;
            }
            return null;
        }

        protected override State GetTransition()
        {
            return null;
        }
    }

    //冰
    public class IcePlatform : InteractionState
    {
        Platform Platform;
        float inertia = 0f;
        Player player;
        public IcePlatform(StateMachine machine, State parent, Platform platform) : base(machine, parent)
        {
            Platform = platform;
        }

        protected override State GetTransition() =>
            Platform.elementEnmu == ElementType.water ? ((PlatformRoot)Parent).water : null;

        protected override void OnEnter()
        {
            Platform.sprite.color = Color.white;
            Platform.collider2d.isTrigger = false;
            //Platform.collider2d.usedByEffector = true;
            player = null;
            Platform.gameObject.layer = Platform.layerIce;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (player)
            {
                if (Mathf.Abs(player.ctx.rb.velocity.x) < Mathf.Abs(inertia))
                {
                    player.ctx.velocity.x = Mathf.MoveTowards(player.ctx.velocity.x,
                        inertia,
                        deltaTime * player.ctx.acceleration * Platform.inertialCoefficient);
                }
                inertia = player.ctx.velocity.x;
            }
        }

        protected override void OnTriggerEnter(Collider2D collision)
        {
            //子弹是水元素
            if (collision.GetComponent<Bullet>() is Bullet collidedBullet)
            {
                if (collidedBullet.enmu == ElementType.water)
                {
                    Platform.elementEnmu = collidedBullet.enmu;
                    Object.Destroy(collision.gameObject);
                    return;
                }
            }

            if (collision.GetComponent<AirWeapon>() is AirWeapon airWeapon)
            {
                if (airWeapon.elementType == ElementType.water)
                {
                    Platform.elementEnmu = airWeapon.elementType;
                    return;
                }
            }

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

    }

    //水
    public class WaterPlatform : InteractionState
    {
        Platform Platform;
        Player player;
        Vector2 direction;


        public WaterPlatform(StateMachine machine, State parent, Platform platform) : base(machine, parent)
        {
            Platform = platform;
        }
        protected override State GetTransition() =>
            Platform.elementEnmu == ElementType.ice && player == null
            ? ((PlatformRoot)Parent).ice : null;

        protected override void OnEnter()
        {
            Platform.sprite.color = Color.blue;
            Platform.collider2d.isTrigger = true;
            //Platform.collider2d.usedByEffector = false;
            player = null;
            Platform.gameObject.layer = Platform.layerWater;
        }

        protected override void OnUpdate(float deltaTime)
        {
            if (player)
            {
                player.ctx.velocity.x = direction.x * Platform.playerSpeed;
                player.ctx.rb.velocity = new(0, direction.y * Platform.playerSpeed);

            }
        }

        protected override void OnTriggerEnter(Collider2D collision)
        {
            if (collision.GetComponent<Bullet>() is Bullet collidedBullet)
            {
                if (collidedBullet.enmu == ElementType.ice)
                {
                    Platform.elementEnmu = collidedBullet.enmu;
                    Object.Destroy(collision.gameObject);
                    return;
                }
            }

            if (collision.GetComponent<AirWeapon>() is AirWeapon airWeapon)
            {
                if (airWeapon.elementType == ElementType.ice)
                {
                    Platform.elementEnmu = airWeapon.elementType;
                    return;
                }
            }


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
}