using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_Game : MonoBehaviour
{
    [SerializeField] private GameObject ultimateFX; //发光特效

    [SerializeField] private Image[] armorUIs;         //护甲 UI 
    [SerializeField] private Image[] luminousArmorUIs; //发光护甲 UI 

    [SerializeField] private GameObject fire50;  //火焰条纹
    [SerializeField] private GameObject fire100;
    [SerializeField] private GameObject fireAnim;

    [SerializeField] private Image[] ultimateEnergyUIs; //大招能量槽 UI 组: 25，50，75，100
    [SerializeField] private GameObject freezeUltimate; //大招冻结槽

    [SerializeField] private RectTransform rectTransform;// 能量槽组件的 RectTransform
    private Vector3 originalPos;            // 原始位置

    public PlayerController player;

    private void Start()
    {
        player = PlayerManager.instance.player;
        freezeUltimate.SetActive(false);
        SetUltimateFX(false);

        // 记录初始位置，防止抖动后位置偏移
        originalPos = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        

    }

   public void SetUltimateFX(bool _bool)//特效控制
   {
        //关闭发光特效
        ultimateFX.SetActive(_bool);
        //关闭火焰动画
        fireAnim.SetActive(_bool);  

        if (!_bool)//如果刚退出大招状态
        {
            //关闭所有火焰装饰
            fire100.SetActive(false);
            fire50.SetActive(false);

        }


        if (_bool)
        {
            player.playerStats.SetArmorRecovery(12);
        }
    }

    public void UpdateArmorUI()//更新护甲UI
    {
        for(int i=0; i < player.playerStats.armor.GetValue(); i++)
        {
            // 受击时，发光特效与护盾光亮同时熄灭
            if (i < player.playerStats.currrentArmor)
            {
                armorUIs[i].color = new Color(armorUIs[i].color.r, armorUIs[i].color.g, armorUIs[i].color.b, 1);
                luminousArmorUIs[i].color = new Color(luminousArmorUIs[i].color.r, luminousArmorUIs[i].color.g, luminousArmorUIs[i].color.b, 1);
            }
            else
            {
                armorUIs[i].color = new Color(armorUIs[i].color.r, armorUIs[i].color.g, armorUIs[i].color.b, 0);
                luminousArmorUIs[i].color = new Color(luminousArmorUIs[i].color.r, luminousArmorUIs[i].color.g, luminousArmorUIs[i].color.b, 0);

            }
        }

    }

    public void UpdateUltimateEnergyUI()//更新大招能量槽UI
    {
        ultimateEnergyUIs[0].fillAmount = (player.playerStats.currrentUltimateEnergy * 4) / 100;
        ultimateEnergyUIs[1].fillAmount = (player.playerStats.currrentUltimateEnergy * 2) / 100;
        ultimateEnergyUIs[2].fillAmount = (player.playerStats.currrentUltimateEnergy * 4) / 300;
        ultimateEnergyUIs[3].fillAmount = (player.playerStats.currrentUltimateEnergy ) / 100;

        if (player.playerStats.currrentUltimateEnergy < 100)
            fire100.SetActive(false);
        else
        {
            fire100.SetActive(true);
            fireAnim.SetActive(true);
        }

        if (player.playerStats.currrentUltimateEnergy < 50)
            fire50.SetActive(false);
        else
            fire50.SetActive(true);
    }


    public void StartFreezeUltimate() => freezeUltimate.SetActive(true);
    public void CloseFreezeUltimate() => freezeUltimate.SetActive(false);




    #region    释放大招失败的抖动效果
    public void EnergyUIShake()
    {
        // 启动一个抖动协程
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float duration = 0.3f;     // 抖动持续时间（秒）
        float time = 0f;           // 计时器
        float magnitude = 10f;     // 抖动幅度（像素）

        while (time < duration)
        {
            // Random.insideUnitCircle：随机生成一个二维方向，模拟自然抖动，乘以幅度
            Vector2 offset = Random.insideUnitCircle * magnitude;

            // 应用偏移量到能量槽的位置
            rectTransform.anchoredPosition = originalPos + new Vector3(offset.x, offset.y, 0);

            // 更新时间
            time += Time.deltaTime;

            // 等待下一帧
            yield return null;
        }

        // 抖动结束，恢复原始位置
        rectTransform.anchoredPosition = originalPos;
    }




    #endregion

    #region    释放大招失败的闪烁效果（改成抖动，原代码留下备用）
    /* [SerializeField] private GameObject Ultimateflash;  //释放大招失败的闪烁效果
    public void UltimateUIFlash()
    {
        StartCoroutine(UltimateUIFlahFX());
    }

    private IEnumerator UltimateUIFlahFX()
    {
        InvokeRepeating("UltimateUIFX", 0, 0.1f);

        yield return new WaitForSeconds(0.5f);

        CancelInvoke("UltimateUIFX");
        Ultimateflash.SetActive(false);

    }

    private void UltimateUIFX() //闪烁效果
    {
        if (Ultimateflash.activeSelf)
            Ultimateflash.SetActive(false);
        else
            Ultimateflash.SetActive(true);
    }*/
    #endregion
}
