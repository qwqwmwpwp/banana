using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;
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
    [Header("黑屏设置")]
    public Image fadeImage;         // 用于黑屏的UI Image
    public float fadeDuration = 1f; // 渐变时间

    public UI_Note Note;
    public UI_Game GameUI;

    public GameObject hd; // 测试红点

    private void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        FadeOut();
        // 确保初始透明
        fadeImage.color = new Color(0, 0, 0, 0);

    }

    int i = 0;//临时测试用

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!Note.gameObject.activeSelf)
            {
                Note.gameObject.SetActive(true);
                Note.InitializeMenus();
                PauseGame();

            }
            else
            {
                Note.InitializeMenus();
                Note.gameObject.SetActive(false);
                ResumeGame();
            }
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("解锁页面测试");
            Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(i);
            FadeInOut(null);
            i++;
        }
    }

    /// <summary>
    /// DOFade（alpha 值，动画持续时间）
    /// SetEase（Ease 类型）：设置动画的缓动曲线 Linear（匀速）、InBounce（开头弹跳）、OutElastic（结束弹性）、InOutQuad（自然）
    /// <summary>
    private void FadeIn(Action onComplete = null)
    {
        fadeImage.DOFade(1f, fadeDuration)
                 .SetEase(Ease.InOutQuad)
                 .OnComplete(() => onComplete?.Invoke());
    }

    private void FadeOut(Action onComplete = null)
    {
        fadeImage.DOFade(0f, fadeDuration)
                 .SetEase(Ease.InOutQuad)
                 .OnComplete(() => onComplete?.Invoke());
    }

    public void FadeInOut(Action middleAction)
    {
        // 在 FadeIn 中传入一个委托，诺存在则调用，否则直接FadeOut
        FadeIn(() =>
        {
            middleAction?.Invoke();
            FadeOut();
        });
    }
   
    // 暂停游戏
    public void PauseGame()
    {
        Time.timeScale = 0f; // 暂停时间
        Debug.Log("游戏已暂停");
    }

    // 恢复游戏
    public void ResumeGame()
    {
        Time.timeScale = 1f; // 恢复时间
        Debug.Log("游戏已恢复");
    }




}
