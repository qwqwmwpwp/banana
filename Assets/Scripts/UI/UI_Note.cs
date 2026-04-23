using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Note : MonoBehaviour
{
    [SerializeField] Button mechanismButton; // 机制按钮
    [SerializeField] Button missionsButton;  // 任务按钮
    [SerializeField] Button characterButton; // 角色按钮
    [SerializeField] Button plotsButton;     // 剧情按钮
    [SerializeField] Button entriesButton;   // 词条按钮



    public GameObject mechanism;
    public GameObject missions;
    public GameObject character;
    public GameObject plots;
    public GameObject entries;

    private bool[] bt;                        // 保存按钮选中状态
    private List<Button> buttons = new List<Button>();
    private List<GameObject> menus = new List<GameObject>();
    private Vector2[] originalPos;            // 记录初始位置
    private Vector3[] originalScale;          // 保存初始缩放

    private void Awake()
    {
        // 添加按钮到列表
        buttons.Add(mechanismButton);
        buttons.Add(missionsButton);
        buttons.Add(characterButton);
        buttons.Add(plotsButton);
        buttons.Add(entriesButton);

        menus.Add(mechanism);
        menus.Add(missions);
        menus.Add(character);
        menus.Add(plots);
        menus.Add(entries);

        int count = buttons.Count;
        bt = new bool[count];
        originalPos = new Vector2[count];
        originalScale = new Vector3[count];

        // 保存初始位置与缩放，并绑定点击事件
        for (int i = 0; i < count; i++)
        {
            int index = i;

            RectTransform rt = buttons[i].GetComponent<RectTransform>(); // 使用 RectTransform
            originalPos[i] = rt.anchoredPosition;                        // 使用 anchoredPosition 而不是 localPosition
            originalScale[i] = rt.localScale;

            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        InitializeMenus(); // 初始化菜单
    }

    public void InitializeMenus() // 初始化菜单
    {
        OnButtonClick(0);
    }

    private void OnButtonClick(int index)
    {
        if (index < 0 || index >= bt.Length)
        {
            Debug.LogError("OnButtonClick index 超出范围: " + index);
            return;
        }
        if (bt[index]) return; // 已经是选中状态，不再执行

        if (index == 0)        // 如果打开机制页面，让其回到第一页
            mechanism.GetComponent<UI_Mechanism>().Initialize();

        // 设置按钮状态与切换对应页面
        for (int i = 0; i < buttons.Count; i++)
        {
            bt[i] = (i == index);

            RectTransform buttonRect = buttons[i].GetComponent<RectTransform>(); // 获取 RectTransform

            if (i == index)
            {
                menus[i].SetActive(true);

                // 使用 DOAnchorPosX 而不是 DOLocalMoveX
                buttonRect.DOAnchorPosX(originalPos[i].x + 50, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);
                buttonRect.DOScale(originalScale[i] * 1.2f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
            }
            else
            {
                menus[i].SetActive(false);

                // 使用 DOAnchorPosX 而不是 DOLocalMoveX
                buttonRect.DOAnchorPosX(originalPos[i].x, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);
                buttonRect.DOScale(originalScale[i], 0.2f).SetEase(Ease.OutBack).SetUpdate(true);
            }
        }
    }
}
