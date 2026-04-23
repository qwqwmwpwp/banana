using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// 此脚本挂在触发器上，用于过场动画的触发

public class ActivateCutscene : MonoBehaviour
{
    //控制 Timeline 播放的组件
    [SerializeField] private PlayableDirector PlayableDirector;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            PlayableDirector.Play();
            Destroy(gameObject);
        }
    }
}
