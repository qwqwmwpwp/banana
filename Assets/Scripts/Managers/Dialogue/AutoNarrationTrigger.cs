using UnityEngine;

public class AutoNarrationTrigger : MonoBehaviour
{
    private DialogueManager dialogueManager;

    [Header("旁白设置")]
    [TextArea(1, 3)]
    [SerializeField] private string[] narrationLines; // 旁白内容

    [SerializeField] private string narratorName;   // 旁白名字
    [SerializeField] private Sprite narratorImage;  // 旁白头像

    private bool hasTriggered = false; // 是否已经触发过（只触发一次）

    private void Start()
    {
        dialogueManager = DialogueManager.instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查进入的是玩家并且没触发过
        if (collision.GetComponent<PlayerController>() != null && !hasTriggered)
        {
            hasTriggered = true; // 标记为已触发
            TriggerNarration();
        }
    }

    private void TriggerNarration()
    {
        // 自动播放旁白
        dialogueManager.SetDialogue(narrationLines, narratorName, narratorImage, false);
    }
}
