using HSM;
using qwq;
using Unity.VisualScripting;
using UnityEngine;

namespace qwq
{
    public class Ball : MonoBehaviour, IInteraction
    {
        HSM.StateMachine stateMachine;
        private BallRoot rootState;

        public ElementType elementEnmu; // 元素类型
        [HideInInspector] public SpriteRenderer sprite;
        [HideInInspector] public Rigidbody2D rb;

        public float iceSpeed = 4f;// 冰元素速度

        public void Awake()
        {
            sprite = GetComponent<SpriteRenderer>();
            rb = GetComponent<Rigidbody2D>();

            rootState = new BallRoot(stateMachine, this);
            StateMachineBuilder builder = new StateMachineBuilder(rootState);
            stateMachine = builder.Build();
        }

        private void Update()
        {
            stateMachine.Tick(Time.deltaTime);

        }

        public void Trigger(GameObject gObj)
        {
            ElementType newElementEnmu = elementEnmu;

            if (gObj.GetComponent<Bullet>() is Bullet bullet)
            {
                if (bullet.enmu != ElementType.water && bullet.enmu != ElementType.ice)
                    return;

                newElementEnmu = bullet.enmu;
                //Destroy(bullet.gameObject);
            }

            if (gObj.GetComponent<AirWeapon>() is AirWeapon airWeapon)
            {
                if (airWeapon.elementType != ElementType.water && airWeapon.elementType != ElementType.ice)
                    return;

                newElementEnmu = airWeapon.elementType;
            }

            elementEnmu = newElementEnmu;
        }

        public void OnTriggerEnter2D(Collider2D collision)
        {
            rootState.TriggerEnter(collision);
        }
    }
}

namespace HSM
{
    public class BallRoot : InteractionState
    {
        Ball Ball;
        IceBall ice;
        WaterBall water;

        public BallRoot(StateMachine machine, Ball ball) : base(machine, null)
        {
            ice = new IceBall(machine, this, ball);
            water = new WaterBall(machine, this, ball);
            this.Ball = ball;
        }

        protected override State GetInitialState()
        {
            switch (Ball.elementEnmu)
            {
                case ElementType.water: return water;
                case ElementType.ice: return ice;
            }
            return null;
        }

        protected override State GetTransition()
        {
            InteractionState newState = null;
            switch (Ball.elementEnmu)
            {
                case ElementType.water:
                    newState = water;
                    break;
                case ElementType.ice:
                    newState = ice;
                    break;
            }
            if (newState != ActiveChild) return newState;

            return null;
        }

    }

    internal class WaterBall : InteractionState
    {
        private Ball ball;

        public WaterBall(StateMachine machine, State parent, Ball ball) : base(machine, parent)
        {
            this.ball = ball;
        }

        protected override void OnEnter()
        {
            ball.sprite.color = Color.blue;
        }

        protected override void OnTriggerEnter(Collider2D collision)
        {
            if (collision.GetComponent<Player>() is Player player)
            {
                Vector2 direction = PlayerTools.Direction_8(ball.transform.position, player.ctx.transform.position);
                player.ctx.velocity.x = direction.x * 15;
                player.ctx.rb.velocity = new(player.ctx.velocity.x, direction.y * 15);
            }

            if (collision.GetComponent<Bullet>() is Bullet collidedBullet
         && collidedBullet.enmu == ElementType.water)
            {
                Vector2 AxisSymmetry = PlayerTools.Direction_8(ball.transform.position, collidedBullet.transform.position);
                Vector2 direction = 2 * AxisSymmetry - collidedBullet.rb.velocity.normalized;
                collidedBullet.rb.velocity = direction * collidedBullet.v;
            }

            if (collision.GetComponent<Ball>() is Ball coball
                && coball.elementEnmu == ElementType.ice)
            {
                Vector2 AxisSymmetry = PlayerTools.Direction_8(ball.transform.position, coball.transform.position);
                Vector2 direction = 2 * AxisSymmetry - coball.rb.velocity.normalized;
                coball.rb.velocity = direction * coball.iceSpeed;

            }
        }

        

    }

    public class IceBall : InteractionState
    {
        private Ball ball;

        public IceBall(StateMachine machine, State parent, Ball ball) : base(machine, parent)
        {
            this.ball = ball;

        }

        protected override void OnEnter()
        {
            ball.sprite.color = Color.white;
        }

        protected override void OnTriggerEnter(Collider2D collision)
        {


            if (collision.GetComponent<Platform>() is Platform platform)
            {
                if (platform.elementEnmu != ElementType.ice) return;

                // 获取球的当前速度向量
                Vector2 ballVelocity = ball.rb.velocity;
                // 计算速度向量的角度（0-360度，0度指向右）
                float velocityAngle = Mathf.Atan2(ballVelocity.y, ballVelocity.x) * Mathf.Rad2Deg;
                // 获取平台的旋转
                Quaternion platformRotation = platform.transform.rotation;
                // 计算平台的上方向（考虑旋转后的Y轴方向）
                Vector3 platformUpDirection = (platformRotation * Vector3.up).normalized;

                // 计算速度方向与平台方向的夹角差（0-180度）
                float angleDifference = Mathf.Abs(platform.transform.eulerAngles.z + 90 - velocityAngle) % 360;
                // 将角度差归一化到0-180度
                angleDifference = angleDifference > 180 ? 360 - angleDifference : angleDifference;
                // 如果角度差超过90度，需要反向
                if (angleDifference >= 90) platformUpDirection *= -1;

                // 将球的速度设置为平台方向的速度
                ball.rb.velocity = platformUpDirection * ball.iceSpeed;

                // 销毁碰撞对象
                Object.Destroy(collision.gameObject);
                return;

            }

            //子弹是冰元素
            if (collision.GetComponent<Bullet>() is Bullet collidedBullet
                && collidedBullet.enmu == ElementType.ice)
            {
                // 将球的速度设置为子弹方向乘以冰速度
                ball.rb.velocity = collidedBullet.direction * ball.iceSpeed;
                // 销毁冰元素子弹
                Object.Destroy(collidedBullet.gameObject);

                return;
            }

            //球碰撞
            if (collision.GetComponent<Ball>() is Ball collidedBall
                &&collidedBall.elementEnmu==ElementType.ice)
            {
                // 如果当前球速度过小，不处理碰撞
                if (ball.rb.velocity.magnitude < 0.1f) return;

                // 如果碰撞的球有较大速度，两者都销毁（弹性碰撞）
                if (collidedBall.rb.velocity.magnitude > 0.1f)
                {
                    Object.Destroy(collidedBall.gameObject);
                    Object.Destroy(ball.gameObject);
                    return;
                }

                // 如果碰撞的球静止，将当前球的速度传递给它
                collidedBall.rb.velocity = ball.rb.velocity;
                Object.Destroy(ball.gameObject);
            }
        }
    }


}
