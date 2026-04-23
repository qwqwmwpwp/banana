using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    private bool isTimelineDialogue;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;


    [SerializeField] private Image ImageForPlayer;//主角图像
    private string nameForPlayer; //主角名字

    [SerializeField] private Image ImageForNpc;   //npc图像
    private string nameForNpc;    //npc名字
    
    // 缓存原始缩放，在 Start 保存一次
    private Vector3 playerOriginalScale;
    private Vector3 npcOriginalScale;

    public bool isDialogue;      //正在对话
    private bool isScrolling;    //正在滚动
    private int currentLines;    //对话索引
    [SerializeField] private float ScrollingSpeed;//滚动字幕时间间隔

    [TextArea(1, 3)]
    [SerializeField] public string[] dialogueLines; //对话内容


    public static DialogueManager instance;//单例
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
    private void Start()
    {
        playerOriginalScale = ImageForPlayer.rectTransform.localScale;
        npcOriginalScale = ImageForNpc.rectTransform.localScale;

        if (!string.IsNullOrWhiteSpace(dialogueText.text))
            dialogueText.text = dialogueLines[currentLines];

        ScrollingSpeed = 0.01f;

        nameForPlayer = "灯";
    }

    private void Update()
    {
        if (dialoguePanel.activeSelf && !isTimelineDialogue)//如果对话窗口为显示状态且不为 Timeline 对话
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isScrolling)
            {
                ScrollingSpeed = 0.01f;
                if (currentLines < dialogueLines.Length - 1)
                {
                    currentLines++;
                    ChackName();

                    AudioManager.instance.PlaySFX(27);

                    //调用携程逐字输出
                    StartCoroutine(TextScrolling(dialogueLines[currentLines]));
                }
                else
                {
                    //清空 对话内容 与 任务脚本
                    CloseDialogue();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && isScrolling)
            {
                ScrollingSpeed = 0.0001f;
            }

        }
    }

    private void SetPortrait(string _name)
    {
        // 恢复初始状态，防止叠加放大
        ResetPortrait();

        float scaleFactor = 1.2f; // 放大比例

        if (_name == nameForNpc)
        {
            Debug.Log("放大 NPC");
            ImageForNpc.rectTransform.localScale = npcOriginalScale * scaleFactor;
            ImageForPlayer.color = new Color(1, 1, 1, 0.5f);
        }
        else if (_name == nameForPlayer)
        {
            Debug.Log("放大主角");
            ImageForPlayer.rectTransform.localScale = playerOriginalScale * scaleFactor;
            ImageForNpc.color = new Color(1, 1, 1, 0.5f);
        }
    }

    private void ResetPortrait()
    {
        // 重置透明度和大小
        ImageForPlayer.rectTransform.localScale = playerOriginalScale;
        ImageForNpc.rectTransform.localScale = npcOriginalScale;

        ImageForPlayer.color = new Color(1, 1, 1, 1);
        ImageForNpc.color = new Color(1, 1, 1, 1);
    }

    private void ChackName()
    {
        //StartsWith 是 C# string 类的方法，用于检查字符串是否以特定的前缀开始。
        if (dialogueLines[currentLines].StartsWith("n_"))
        {
            //Replace 是 C# string 类的方法，用于替换字符串中的特定字符或子字符串,在这将所有的 n_ 替换成空值
            nameText.text = "<i><b>" + dialogueLines[currentLines].Replace("n_", "") + "</b></i>";

            if (nameText.text == "<i><b>" + nameForPlayer + "</b></i>")
                SetPortrait(nameForPlayer);//立绘控制

            if (nameText.text == "<i><b>" + nameForNpc + "</b></i>")
                SetPortrait(nameForNpc);


            currentLines++;

            // 再次判断，确保 currentLines 没越界
            if (currentLines >= dialogueLines.Length)
            {
                CloseDialogue();
            }
        }
    }


    //在对话人物调用，将其 defaultDialogueLines 传入 DialogueManager
    public void SetDialogue(string[] _newLines, string _nameForNpc, Sprite _ImageForNpc, bool _isTimelineDialogue)
    {
        // 禁止输入
        PlayerManager.instance.player.playerControls.GamePlay.Disable();

        isTimelineDialogue = _isTimelineDialogue;

        nameForNpc = _nameForNpc;

        ImageForNpc.sprite = _ImageForNpc; // 把 sprite 设置给 UI 的 Image 组件

        dialogueLines = _newLines;
        currentLines = 0;
        ChackName();
        StartCoroutine(TextScrolling(dialogueLines[currentLines]));


        dialoguePanel.SetActive(true);
        isDialogue = true;
    }

    

    public void CloseDialogue()//清空 对话内容 与 任务脚本
    {
        isDialogue = false;

        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(false);
        }

        nameText.text = "";

        // 恢复输入
        PlayerManager.instance.player.playerControls.GamePlay.Enable();

    }

    private IEnumerator TextScrolling(string _text)//用于逐字输出的携程
    {
        isScrolling = true;
        dialogueText.text = "";

        for (int i = 0; i < _text.Length; i++)
        {
            //利用 string 的 += 实现一个字一个字添加
            dialogueText.text += _text[i];
            yield return new WaitForSeconds(ScrollingSpeed);
        }
        isScrolling = false;
    }

    #region    Timeline专用方法
    public void SetDialogueToTimeline(string _Lines, string _name, Sprite _ImageForNpc, bool _isTimelineDialogue)
    {
        isTimelineDialogue = _isTimelineDialogue;

        ImageForNpc.sprite = _ImageForNpc; // 把 sprite 设置给 UI 的 Image 组件

        nameText.text = "<i>" + _name + "</i>";

        if (nameText.text == "<i>" + nameForPlayer + "</i>")
            SetPortrait(nameForPlayer);//立绘控制
        else
            SetPortrait(nameForNpc);

        StartCoroutine(TextScrollingToTimeline(_Lines));

        dialoguePanel.SetActive(true);
        isDialogue = true;
    }

    private IEnumerator TextScrollingToTimeline(string _text)//用于逐字输出的携程
    {
        isScrolling = true;
        dialogueText.text = "";

        for (int i = 0; i < _text.Length; i++)
        {
            //利用 string 的 += 实现一个字一个字添加
            dialogueText.text += _text[i];
            yield return new WaitForSeconds(ScrollingSpeed);
        }
        isScrolling = false;
    }

    #endregion

}
