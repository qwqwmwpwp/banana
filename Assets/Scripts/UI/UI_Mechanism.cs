using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Mechanism : MonoBehaviour //机制界面控制//
{
    [SerializeField] Button PageButton1; // 上一页按钮
    [SerializeField] Button PageButton2; // 下一页按钮

    [SerializeField] List<GameObject> pages; // 页面列表，用于存放所有机制页面
    [SerializeField] List<GameObject> tipss; // 提示列表，用于显示解锁提示
    [SerializeField] GameObject homePage;    // 总页面

    /// <summary>
    /// 测试：
    /// 火0，1，2，3，4
    /// 水5
    /// 地6
    /// 风7
    /// 秩序8
    /// 混沌9
    /// </summary>
    int fireMax = 4;
    int waterMax = 5;
    int earthMax = 6;
    int windMax = 7;
    int orderMax = 8;
    int chaosMax = 9;
    [SerializeField] List<Button> categoryButtons; // 分类按钮列表


    public int CurrentPages = 0;      // 当前页面索引，用于判定翻页
    private int currentCategory = -1; // 当前选中的分类索引

    int tips = -1;         // 已解锁的提示索引
    int unlockPages = -1;  // 已解锁的页面索引，用于判定下一页是否可用

    bool[] tipsViewed = new bool[13];
    // 玩家是否看过该索引的提示

    void Start()
    {
        // 给按钮绑定点击事件
        PageButton1.onClick.AddListener(SetPage1);
        PageButton2.onClick.AddListener(SetPage2);

        // 为分类按钮绑定点击事件
        for (int i = 0; i < categoryButtons.Count; i++)
        {
            int index = i;
            categoryButtons[i].onClick.AddListener(() => OnCategoryButtonClick(index));
        }

        for (int i = 0; i < tipsViewed.Length; i++)
        {
            tipsViewed[i] = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Esc();


    }

    public void Initialize() // 初始化机制界面，每次打开时调用
    {
        // 将所有提示隐藏
        for (int i = tipss.Count - 1; i > tips; i--)
            tipss[i].SetActive(false);

        // 回到总页面
        ReturnToHomePage();
    }


    public void ReturnToHomePage() // 到总页面
    {
        homePage.SetActive(true);
        currentCategory = -1;

        // 隐藏所有子页面
        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].SetActive(false);
        }

        // 隐藏翻页按钮
        PageButton1.gameObject.SetActive(false);
        PageButton2.gameObject.SetActive(false);
    }


    private void OnCategoryButtonClick(int categoryIndex) // 分类按钮点击事件
    {
        // 显示对应分类的页面
        if (categoryIndex == 0 && unlockPages >= 0) // 火关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(0);
        }
        if (categoryIndex == 1 && unlockPages > fireMax) // 水关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(fireMax + 1);
        }
        if (categoryIndex == 2 && unlockPages > waterMax) // 地关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(waterMax + 1);
        }
        if (categoryIndex == 3 && unlockPages > earthMax) // 风关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(earthMax + 1);
        }
        if (categoryIndex == 4 && unlockPages > windMax) // 秩序关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(windMax + 1);
        }
        if (categoryIndex == 5 && unlockPages > orderMax) // 混沌关卡 机制
        {
            EnteCategory(categoryIndex);
            SetPage(orderMax + 1);
        }


    }

    private void EnteCategory(int categoryIndex)
    {
        // 更新索引与关闭总页面
        currentCategory = categoryIndex;
        homePage.SetActive(false);
        // 显示翻页按钮
        PageButton1.gameObject.SetActive(true);
        PageButton2.gameObject.SetActive(true);
    }

    public void UnlockTips(int _i) // 解锁提示，同时根据解锁数量解锁页面
    {
        if (_i > tipss.Count - 1)
        {
            Debug.Log("超出索引异常！");
            return;
        }

        if (tipsViewed[_i] == false)
        {
            // 产生红点
            UIManager.instance.hd.SetActive(true);
        }

        tips = _i; // 更新已解锁数量

        tipss[_i].SetActive(true);

        // 根据解锁提示的数量开放页面
        unlockPages = _i/2;

        Sprite sprite = null;
        PopupManager.instance.ShowItemTips("新的笔迹已解锁", "点击<color=red>[TAB]</color>打开<color=yellow>笔记</color>", "探寻火焰的奥秘,<color=yellow>从曾行于此的你</color>", sprite,  false);
    }

    private void Esc()
    {
        if (currentCategory == -1)
        {
            UIManager.instance.Note.InitializeMenus();
            UIManager.instance.Note.gameObject.SetActive(false);
            UIManager.instance.ResumeGame();
        }
        else
        {
            ReturnToHomePage();
        }
    }

    #region 切换页面
    private void SetPage(int _i)
    {
        // 如果当前在总页面或第一个页面未开启，不执行页面切换
        if (currentCategory == -1 || unlockPages == -1) return;

        for (int i = 0; i < pages.Count; i++)
        {
            if (i == _i && _i <= unlockPages)
                pages[i].SetActive(true);
            else
                pages[i].SetActive(false);
        }

        CurrentPages = _i;

        if (_i == unlockPages) // 如果打开最新一面
        {
            for (int i = 0; i <= _i; i++)
                tipsViewed[i] = true;

            // 消除红点
            UIManager.instance.hd.SetActive(false);
        }
    }

    private void SetPage1() // 切换到上一页
    {
        if (currentCategory == 0 && CurrentPages > 0) // 火关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);
        }
        else if (currentCategory == 1 && CurrentPages > fireMax + 1) // 水关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);
        }
        else if (currentCategory == 2 && CurrentPages > waterMax + 1) // 地关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);
        }
        else if (currentCategory == 3 && CurrentPages > earthMax + 1) // 风关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);
        }
        else if (currentCategory == 4 && CurrentPages > windMax + 1) // 秩序关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);
        }
        else if (currentCategory == 5 && CurrentPages > orderMax + 1) // 混沌关卡 机制
        {
            CurrentPages--;
            SetPage(CurrentPages);

        }
        else
            ReturnToHomePage();


    }

    private void SetPage2() // 切换到下一页
    {
        if (CurrentPages < unlockPages) // 如果未超过已解锁的最大页数
        {
            if (currentCategory == 0 && CurrentPages < fireMax) // 火关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);
            }
            if (currentCategory == 1 && CurrentPages < waterMax) // 水关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);

            }
            if (currentCategory == 2 && CurrentPages < earthMax) // 地关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);

            }
            if (currentCategory == 3 && CurrentPages < windMax) // 风关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);

            }
            if (currentCategory == 4 && CurrentPages < orderMax) // 秩序关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);

            }
            if (currentCategory == 5 && CurrentPages < chaosMax) // 混沌关卡 机制
            {
                CurrentPages++;
                SetPage(CurrentPages);

            }
        }
    }
    #endregion
}
