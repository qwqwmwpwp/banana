using System.Threading;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Lighthouse : MonoBehaviour
{
    public BuffType ThisType;

    Animator anim;

    public GameObject door;

    [Header("UI 引用")]
    public GameObject tips;           // 按 E 提示
    public GameObject tips1;          // 待激活 提示
    public Image progressCircle;      // 圆环进度条
    public GameObject fire;           // 火焰动画
    SpriteRenderer fire_sr;           // 用于火焰更改颜色
    SpriteRenderer sr;                // 用于灯柱更改颜色

    [Header("交互设置")]
    public float holdTime = 3f;  // 需要按住多久才触发

    private bool CanInteraction = false;
    private float holdTimer = 0f;

    public bool isKindled = false; // 是否点燃
    public int kindledTime = 20;   // 普通灯台点燃时间
    public float kindledTimer = 0f;

    private void Start()
    {
        tips.SetActive(false);
        tips1.SetActive(false);
        progressCircle.fillAmount = 0;

        fire.SetActive(false);
        sr = GetComponent<SpriteRenderer>();
        fire_sr = fire.GetComponent<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();

        if (door != null)
            door.SetActive(false);
    }

    private void Update()
    {
        if (CanInteraction) // 允许交互
        {
            if (Input.GetKey(KeyCode.E))
            {
                holdTimer += Time.deltaTime;
                progressCircle.fillAmount = holdTimer / holdTime;

                if (holdTimer >= holdTime)
                {
                    // 交互成功
                    Debug.Log("交互完成！");
                    DoInteraction();

                    // 重置状态
                    holdTimer = 0;
                    progressCircle.fillAmount = 0;
                }
            }
            else
            {
                // 松开键时重置
                if (holdTimer > 0)
                {
                    holdTimer = 0;
                    progressCircle.fillAmount = 0;
                }
            }
        }

        if (isKindled && ThisType == BuffType.引燃灯火 && SceneManager.GetActiveScene().name == "老一关") 
        {
            // 如果对应灯台进入点燃状态
            kindledTimer += Time.deltaTime;
            if (kindledTimer > kindledTime)
            {
                // 退出点燃状态
                isKindled = false;
                fire.SetActive(false);
                EnemyStats[] monsters = FindObjectsOfType<EnemyStats>();
                foreach (EnemyStats m in monsters)
                {
                    m.FullyRestoreArmor(); // 回复所有护甲
                }
               
            }
        }
    }


    private void DoInteraction() // 完成交互逻辑
    {
        AudioManager.instance.PlaySFX(26);


        CanInteraction = false;
        tips.SetActive(false);
        tips1.SetActive(false);

        BuffManager.instance.UseBuff(ThisType); // 将对应buff使用掉

        // 进入点燃状态
        isKindled = true;
        fire.SetActive(true);


        if (ThisType == BuffType.引燃灯火)
        {
            PopupManager.instance.ShowOperationTips("引火燃灯", false);

            if (SceneManager.GetActiveScene().name == "火开门关")
            {
                EnemyStats[] monsters = FindObjectsOfType<EnemyStats>();
                foreach (EnemyStats m in monsters)
                {
                    m.Die(); // 调用死亡函数
                }
                if (door != null)
                    door.SetActive(true);
            }
            if (SceneManager.GetActiveScene().name == "火老一关")
            {
                EnemyStats[] monsters = FindObjectsOfType<EnemyStats>();
                foreach (EnemyStats m in monsters)
                {
                    m.currrentArmor = 0; // 当前护甲设为零
                }
            }
        }

        if(ThisType == BuffType.焦燃之火 || ThisType == BuffType.锋芒之火)
        {
            if(ThisType == BuffType.焦燃之火)
            {
                PopupManager.instance.ShowOperationTips("祭此焦燃", false);

                sr.color = new Color(0.9921f, 0.3882f, 0f, 1f);
                fire_sr.color = new Color(1f, 0.5333f, 0f, 1f);
            }
            if (ThisType == BuffType.锋芒之火)
            {
                PopupManager.instance.ShowOperationTips("祭此锋芒", false);

                sr.color = new Color(0.858f, 0f, 1f, 1f);
                fire_sr.color = new Color(1f, 0.3836f, 0.6823f, 1f);
            }
            bool x = true ;
            Lighthouse[] lighthouses = FindObjectsOfType<Lighthouse>();
            foreach(Lighthouse l in lighthouses)
            {
                // 查找场景中的两个特殊灯台，判断其是否点燃
                if( l.ThisType == BuffType.焦燃之火 || l.ThisType == BuffType.锋芒之火)
                {
                    if (!l.isKindled)
                        x = false;
                }
            }
            if (x)
            {
                BuffManager.instance.fireLeveStages = 2; // 切换阶段
                BuffManager.instance.AddBuff(BuffType.纯净之火);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || isKindled) return; // 只对玩家响应

        if (BuffManager.instance.HasBuff(ThisType))  // 如果持有符合该灯台的Buff
        {
            CanInteraction = true;
            tips.SetActive(true);
        }
        else
        {
            tips1.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CanInteraction = false;
            tips.SetActive(false);
            tips1.SetActive(false);

            progressCircle.fillAmount = 0;
            holdTimer = 0;
        }
    }


}
