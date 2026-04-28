using HSM;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.LowLevel;

namespace qwq
{
    public class Player : MonoBehaviour
    {
        public Rigidbody2D rb;
        public PlayerAnim anim;
        public PlayerControls_q playerControls;// 玩家输入控制器
        public PlayerContext ctx = new PlayerContext();//玩家参数
        [HideInInspector] public static PlayerContext playerctx {  get; private set; }

        State root;
        HSM.StateMachine machine;
        string lastPath;

      [HideInInspector]public Vector2 direction;
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            playerControls = new PlayerControls_q();
            ctx.detection = GetComponent<PlayerDetection>();

            ctx.rb = rb;
            ctx.transform = transform;
            ctx.anim = anim;
            ctx.airWeapon.Initialize(ctx);
            ctx.waterWeapon.Initialize(ctx);
            ctx.iceWeapon.Initialize(ctx);

            playerctx = ctx;

            root = new PlayerRoot(null, ctx);
            StateMachineBuilder builder = new StateMachineBuilder(root);
            machine = builder.Build();
        }

        void Start()
        {
            //stateMove = new StateMove(this);
            //stateIdle = new StateIdle(this);
            //stateJump = new StateJump(this);
            //stateSky= new StateSky(this);
            //state = stateIdle;
            //state.Enter();
        }

        private void OnEnable()
        {
            playerControls.Enable();

        }

        void Update()
        {
            ctx.rb.gravityScale=ctx.basicGravityScale;

            direction = playerControls.GamePlay.Move.ReadValue<Vector2>();

            if (ctx.energy < ctx.energy_max) ctx.energy += 2 * Time.deltaTime;

            ctx.moveInput.x = Mathf.Clamp(direction.x, -1f, 1f);

            Attack();
            Jump();
            MouseDirection();
            SwitchWeapon();
            machine.Tick(Time.deltaTime);

            var pash = StatePath(machine.Root.Leaf());
            if (pash != lastPath)
            {
                Debug.Log("State: " + pash);
                lastPath = pash;
            }
        }

        private void FixedUpdate()
        {
            var v = ctx.rb.velocity;
            v.x = ctx.velocity.x;
            ctx.rb.velocity = v;
            ctx.velocity.x = ctx.rb.velocity.x;
        }
        private void SwitchWeapon()
        {
            if (playerControls.GamePlay.Switch.triggered)
            {
                ctx.isSwitchWeapon = true;
            }

            if (ctx.swich_t > 0)
            {
                ctx.swich_t -= Time.deltaTime;
            }
        }

        static string StatePath(State s)
        {
            return string.Join(">", s.PathToRoot().Reverse().Select(n => n.GetType().Name));
        }

        private void Jump()
        {
            if (playerControls.GamePlay.Jump.triggered)
            {
                ctx.jumpPressed = true;
            }
            else
            {
                ctx.jumpPressed = false;
            }

            if (ctx.detection.isGrounded || ctx.detection.isPlatform)
            {
                ctx.canDoubleJump = true;
            }
           
        }

        private void Attack()
        {
            if (playerControls.GamePlay.Attack.IsPressed())
            {
                ctx.isAttackInput = true;
            }
            else
            {
                ctx.isAttackInput = false;
            }
        }

        private void MouseDirection()//鼠标方向
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ctx.mouseWorldPos = (mouseWorldPos - (Vector2)transform.position).normalized;
            ctx.mouseWorldPos_8 = PlayerTools.Direction_8((Vector2)transform.position+ctx.detection.center, mouseWorldPos);
        }

    }

    [Serializable]
    public class PlayerContext
    {
        public Rigidbody2D rb;
        public PlayerAnim anim;
        public Transform transform;
        public PlayerDetection detection;


        [Header("跳跃")]
        public float jumpSpeed = 7f;// 跳跃初始速度
        public float jumpInertiaTime = 0.1f;// 跳跃惯性持续时间
        public bool jumpPressed;// 跳跃按键按下状态
        public bool canDoubleJump = true;// 是否允许二段跳
        public float basicGravityScale = 3f;// 基础重力

        [Header("移动")]
        public Vector2 moveInput;// 移动输入方向
        public Vector2 velocity;// 当前速度
        public float acceleration = 40f;// 加速度
        public float moveSpeed = 6f;// 最大移动速度

        [Header("攻击")]
        public ElementType elementType;// 当前元素类型
        public Weapon activeWeapon;// 当前激活的武器
        public IceWeapon iceWeapon;// 冰元素武器
        public WaterWeapon waterWeapon;// 水元素武器
        public Weapon airWeapon; // 空中武器
        public bool isAttackInput;//攻击输入

        [Header("能量")]
        public float energy_max = 8f;
        public float energy = 8f;

        [Header("攻击切换")]
        public bool isSwitchWeapon;
        public float swich_t;
        public float swichSpeed = 4;

        [Header("鼠标")]
        public Vector2 mouseWorldPos;
        public Vector2 mouseWorldPos_8;


        public bool EnergyValueUPdate(float newEnergy)
        {
            if (newEnergy > 0)
            {
                energy += newEnergy;
                if (energy > energy_max) this.energy = energy_max;

                return true;
            }


            if (newEnergy < 0 && energy + newEnergy > 0)
            {
                energy += newEnergy;
                return true;
            }


            return false;
        }
    }

    [Serializable]
    public class PlayerAnim
    {
        [Header("动画信息")]
        public SkeletonAnimation animContorller;
        public Color originalColor;
        [Header("普通状态动画")]
        public AnimationReferenceAsset idleAnim; // 闲置
        public AnimationReferenceAsset moveAnim; // 跑步
        public AnimationReferenceAsset jumpAnim; // 起跳
        public AnimationReferenceAsset deadAnim; // 死亡
        public AnimationReferenceAsset dashAnim; // 冲刺
        public AnimationReferenceAsset hitAnim;  // 受击
        public AnimationReferenceAsset airAnim;  // 空中状态
        public AnimationReferenceAsset interactAnim;     // 交互
        public AnimationReferenceAsset airAttackAnim;    // 空中攻击
        public AnimationReferenceAsset chargeAttackAnim; // 普通蓄力近战
        public AnimationReferenceAsset rangedAttackAnim; // 地面远程攻击
        public AnimationReferenceAsset switchAnim;
        public AnimationReferenceAsset[] attackAnims;    // 地面近战连段
        [Header("大招状态动画")]
        public AnimationReferenceAsset ultimateAnim;      // 进入大招
        public AnimationReferenceAsset ultimateIdleAnim;  // 大招闲置
        public AnimationReferenceAsset ultimateMoveAnim;  // 大招跑步
        public AnimationReferenceAsset ultimateJumpAnim;  // 大招起跳
        public AnimationReferenceAsset ultimateDeadAnim;  // 大招死亡
        public AnimationReferenceAsset ultimateDashAnim;  // 大招冲刺
        public AnimationReferenceAsset ultimateHitAnim;   // 大招受击
        public AnimationReferenceAsset ultimateAirAnim;   // 大招空中状态
        public AnimationReferenceAsset ultimateAirAttackAnim; // 大招空中攻击
        public AnimationReferenceAsset ultimateInteractAnim;  // 大招交互
        public AnimationReferenceAsset[] ultimateAttackAnims; // 大招近战连段
        public TrackEntry SetAnim(AnimationReferenceAsset _anim, bool _loops)
        {

            // 用于切换动画
            TrackEntry entry = animContorller.AnimationState.SetAnimation(0, _anim, _loops);

            // TrackTime: 动画已经播放了多少时间,设置成 0f，把动画时间重置到开头，
            entry.TrackTime = 0f;

            //MixDuration 表示动画和上一个动画之间混合（过渡）的时间长度，设置成 0f，不要任何过渡，立刻切换。
            entry.MixDuration = 0f;

            return entry;
        }
    }

    public class PlayerTools
    {
        public static Vector2 Direction_8(Vector2 form, Vector2 to)
        {

            Vector2 direction = (to - form).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            int angle_abs = 0;
            if (angle < 0) 
                angle_abs = -1;
            else 
                angle_abs = 1;

            int x = (int)(Mathf.Abs(angle) + 22.5) / 45;
            float radians = angle_abs * x * 45 * Mathf.Deg2Rad;
            Vector2 direction_8 = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)).normalized;
            return direction_8;
        }
    }
}