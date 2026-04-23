using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveStele : MonoBehaviour
{
    public float stayTime;
    public List<GameObject> runes;
    private Coroutine coroutine;
    private bool isShow;
    private int num;
    private int index = 0;  //当前符文序列
    private WaitForSeconds t;
    private void Start()
    {
        num = runes.Count;
        isShow = false;
        t = new WaitForSeconds(stayTime / num);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(isShow) return;
        if(coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(ShowRunes());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(isShow) return;
        if(coroutine!=null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(HindeRunes());
    }
    IEnumerator HindeRunes()
    {
        for (int i = index; i >=0; i--)
        {
            runes[i].SetActive(false);
            index = i;
            yield return  t;
        }
       
    }
    IEnumerator ShowRunes()
    {
        for (int i = 0; i < num; i++)
        {
            runes[i].SetActive(true);
            index = i;
            yield return  t;
        }
        isShow = true;
        BuffManager.instance.fireLeveStages = 1;
        PopupManager.instance.ShowOperationTips("携火永铭", false);
    }
}
