using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirwallTrigger : MonoBehaviour
{
    private bool isEnter;
    [Header("血条UI")]
    public GameObject UI_healthBar;
    [Header("空气墙")]
    public List<GameObject> airwalls;

    private void Start()
    {
        isEnter = false;
        for (int i = 0; i < airwalls.Count; i++)
        {
            airwalls[i].SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") &&  !isEnter)
        {
            isEnter = true;
            //生成空气墙
            for (int i = 0; i < airwalls.Count; i++)
            {
                airwalls[i].SetActive(true);
            }
            //播放音效
            AudioManager.instance.ChangeBGM(2);
            //显示血条
            UI_healthBar.SetActive(true);
        }
    }

    public void StopWall()
    {
        for (int i = 0; i < airwalls.Count; i++)
        {
            airwalls[i].SetActive(false);
        }
    }
}
