using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Header("场景提示 UI")]
    [SerializeField] private GameObject enterSceneTips;   // 进入场景提示整体
    [SerializeField] private TextMeshProUGUI sceneName;   // 场景名字

    [Header("物品收集 UI")]
    [SerializeField] private GameObject itemCollectionTips;   // 物品收集提示整体
    [SerializeField] private TextMeshProUGUI itemName;        // 物品名字
    [SerializeField] private TextMeshProUGUI itemDescription; // 物品介绍
    [SerializeField] private TextMeshProUGUI smallTextTip;    // 小字提示
    [SerializeField] private Image itemImage;                 // 物品图像

    [Header("操作提示 UI")]
    [SerializeField] private GameObject operationTips;     // 操作提示整体
    [SerializeField] private TextMeshProUGUI operation;    // 对应操作

    [Header("动画设置")]
    [SerializeField] private float fadeDuration = 0.5f;    // 淡入淡出时间
    [SerializeField] private CanvasGroup canvasGroup;      // 弹窗的整体透明度控制

    [Header("显示模式设置")]
    [SerializeField] private KeyCode closeKey = KeyCode.Space; // 按键关闭时使用的键位
    [SerializeField] private float autoCloseDuration = 5f;     // 自动消失的停留时间

    private Coroutine currentRoutine; // 当前运行中的协程，避免多个同时播放

    private void Start()
    {
        // 初始隐藏
        canvasGroup.alpha = 0;
        enterSceneTips.SetActive(false);
        itemCollectionTips.SetActive(false);
        operationTips.SetActive(false);

        fire1.SetActive(false);
        fire2.SetActive(false);
        buff1.SetActive(false);
        buff2.SetActive(false);


    }

    /// <summary>
    /// 显示场景提示
    /// (场景名字， 是否按下空格键才消失)
    /// </summary>
    public void ShowSceneTips(string sceneText, bool useKeyToClose)
    {
        sceneName.text = sceneText;
        ActivatePopup(enterSceneTips, useKeyToClose);
        AudioManager.instance.PlaySFX(28);

    }

    /// <summary>
    /// 显示物品收集提示
    /// （物品名字， 物品介绍， 物品图像， 是否按下空格键才消失）
    /// </summary>
    public void ShowItemTips(string name, string description, string _smallTextTip, Sprite sprite, bool useKeyToClose)
    {
        itemName.text = name;
        itemDescription.text = description;
        smallTextTip.text = _smallTextTip;
        if (sprite != null)
            itemImage.sprite = sprite;

        ActivatePopup(itemCollectionTips, useKeyToClose);

        AudioManager.instance.PlaySFX(29);

    }

    /// <summary>
    /// 显示操作提示
    /// （操作提示， 是否按下空格键才消失）
    /// </summary>
    public void ShowOperationTips(string operationText, bool useKeyToClose)
    {
        operation.text = operationText;
        ActivatePopup(operationTips, useKeyToClose);
        AudioManager.instance.PlaySFX(29);

    }

    /// <summary>
    /// 根据模式决定关闭逻辑
    /// </summary>
    private void ActivatePopup(GameObject target, bool useKeyToClose)
    {
        // 只显示一个提示
        enterSceneTips.SetActive(false);
        itemCollectionTips.SetActive(false);
        operationTips.SetActive(false);

        target.SetActive(true);

        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(ShowAndHide(useKeyToClose));
    }

    private IEnumerator ShowAndHide(bool useKeyToClose)
    {
        // 淡入
        yield return Fade(0, 1);

        if (useKeyToClose)
        {
            // 模式2：按键关闭
            yield return new WaitUntil(() => Input.GetKeyDown(closeKey));
        }
        else
        {
            // 模式1：自动消失
            yield return new WaitForSeconds(autoCloseDuration);
        }

        // 淡出
        yield return Fade(1, 0);

        // 关闭所有提示
        enterSceneTips.SetActive(false);
        itemCollectionTips.SetActive(false);
        operationTips.SetActive(false);

        currentRoutine = null;
    }

    private IEnumerator Fade(float from, float to)
    {
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = to;
    }




    #region 火焰与buff收集提示
    [Header("火焰收集")]
    public GameObject fire1;
    public TextMeshProUGUI fireName1;
    public TextMeshProUGUI fireNumber1;
    public TextMeshProUGUI fireTimer1;

    public GameObject fire2;
    public TextMeshProUGUI fireName2;
    public TextMeshProUGUI fireNumber2;
    public TextMeshProUGUI fireTimer2;

    /// <summary>
    ///  C# 值类型（如 int, float, enum 等）默认是不能为 null 的
    /// 需要表示值类型可能没有值的情况时,使用可空类型,语法是在值类型后面加上一个问号（?）
    /// </summary>
    private FireType? fire1Type = null; // 跟踪fire1显示的火焰类型
    private FireType? fire2Type = null; // 跟踪fire2显示的火焰类型


    [Header("BUFF收集")]
    public GameObject buff1;
    public TextMeshProUGUI buffName1;
    public TextMeshProUGUI buffTimer1;

    public GameObject buff2;
    public TextMeshProUGUI buffName2;
    public TextMeshProUGUI buffTimer2;

    private BuffType? buff1Type = null;
    private BuffType? buff2Type = null;
    public void SetFire(FireType fireType, int fireNumber, int fireTimer)
    {
        // 如果fire1没有显示任何火焰，或者正在显示同一种火焰
        if (fire1Type == null || fire1Type == fireType)
        {
            fire1Type = fireType;
            fire1.SetActive(true);
            fireName1.text = fireType.ToString();
            fireNumber1.text = fireNumber.ToString();
            StartCountdown(fireTimer, fireTimer1,
                () => { fire1.SetActive(false); fire1Type = null; }
            );
        }

        // 如果fire2没有显示任何火焰，或者正在显示同一种火焰
        else if (fire2Type == null || fire2Type == fireType)
        {
            fire2Type = fireType;
            fire2.SetActive(true);
            fireName2.text = fireType.ToString();
            fireNumber2.text = fireNumber.ToString();
            StartCountdown(fireTimer, fireTimer2,
                () => { fire2.SetActive(false); fire2Type = null; }
            );
        }

    }
    public void CloseFire(FireType fireType)
    {
        if (fire1Type == fireType)
        {
            fire1.SetActive(false);
            fire1Type = null;
            // 停止对应的倒计时协程
            if (activeCoroutines.TryGetValue(fireTimer1, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(fireTimer1);
            }
        }

        if (fire2Type == fireType)
        {
            fire2.SetActive(false);
            fire2Type = null;
            // 停止对应的倒计时协程
            if (activeCoroutines.TryGetValue(fireTimer2, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(fireTimer2);
            }
        }
    }

    public void SetBuff(BuffType buffType, int buffTimer)
    {
        if (buff1Type == null)
        {
            buff1Type = buffType;
            buff1.SetActive(true);
            buffName1.text = buff1Type.ToString();

            if (BuffManager.instance.fireLeveStages == 2)
            {
                buffTimer1.text = "";
            }
            else
            {
                StartCountdown(buffTimer, buffTimer1,
                    () => { buff1.SetActive(false); buff1Type = null; }
                    );
            }

        }
        else if (buff2Type == null)
        {
            buff2Type = buffType;
            buff2.SetActive(true);
            buffName2.text = buff2Type.ToString(); if (BuffManager.instance.fireLeveStages == 2)
            {
                buffTimer2.text = "";
            }
            else
            {
                StartCountdown(buffTimer, buffTimer2,
               () => { buff2.SetActive(false); buff2Type = null; }
               );
            }

        }
    }

    public void CloseBuff(BuffType buffType)
    {
        if (buff1Type == buffType)
        {
            buff1.SetActive(false);
            buff1Type = null;
            // 停止对应的倒计时协程
            if (activeCoroutines.TryGetValue(buffTimer1, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(buffTimer1);
            }
        }

        if (buff2Type == buffType)
        {
            buff2.SetActive(false);
            buff2Type = null;
            // 停止对应的倒计时协程
            if (activeCoroutines.TryGetValue(buffTimer2, out Coroutine coroutine))
            {
                StopCoroutine(coroutine);
                activeCoroutines.Remove(buffTimer2);
            }
        }
    }

    // 用于存储正在运行的协程
    private Dictionary<TextMeshProUGUI, Coroutine> activeCoroutines = new Dictionary<TextMeshProUGUI, Coroutine>();


    private void StartCountdown(int currentTime, TextMeshProUGUI timer, System.Action onComplete = null)
    {
        // 停止同一UI元素上可能正在运行的旧协程
        if (activeCoroutines.ContainsKey(timer))
        {
            StopCoroutine(activeCoroutines[timer]);
            activeCoroutines.Remove(timer);
        }

        // 启动新协程并存储引用
        var coroutine = StartCoroutine(CountdownCoroutine(currentTime, timer, onComplete));
        activeCoroutines[timer] = coroutine;
    }

    IEnumerator CountdownCoroutine(int currentTime, TextMeshProUGUI timer, System.Action onComplete = null)
    {
        while (currentTime > 0)
        {
            timer.text = currentTime.ToString();
            yield return new WaitForSeconds(1f);
            currentTime--;
        }

        onComplete?.Invoke();

        // 从活动协程字典中移除
        if (activeCoroutines.ContainsKey(timer))
        {
            activeCoroutines.Remove(timer);
        }
    }
    #endregion

}