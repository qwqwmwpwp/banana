using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueTrigger : MonoBehaviour
{
    private DialogueManager dialogueManager;
    [SerializeField] private string nameForNpc; //npc名字
    [SerializeField] private Sprite ImageForNpc; //npc图像


    private bool isEnter;    // 是否进入对话范围
    private bool hasTalked;  // 已触发对话标志
    [TextArea(1, 3)]
    [SerializeField] public string[] defaultDialogueLines;   //默认对话内容

    [SerializeField] private GameObject tips;//提示图像


    private void Start()
    {
        dialogueManager = DialogueManager.instance;
    }

    private void Update()
    {
        //如果进入触发器范围且未开始对话，按下 S 调用 SetDialogue
        if (isEnter && !hasTalked && Input.GetKeyDown(KeyCode.S) && !DialogueManager.instance.isDialogue)
        {
            tips.SetActive(false);
            // 标记已经触发
            hasTalked = true;
            //传入默认触发的对话内容
            dialogueManager.SetDialogue(defaultDialogueLines, nameForNpc, ImageForNpc, false);
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            isEnter = true;
            tips.SetActive(true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            isEnter = false;
            hasTalked = false; // 离开范围后可重新触发
            tips.SetActive(false);
            DialogueManager.instance.CloseDialogue();
        }
    }

   
}
