using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public enum TimelineMode
{
    Play,  // 播放
    Pause  // 暂停
}

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager instance;//单例
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
        timelineMode = TimelineMode.Play;
    }

    public TimelineMode timelineMode;

    private PlayableDirector currentPlayableDirector; // 当前激活的动画资源
    public bool isLastClip = false;                     // 是否是动画中最后一个对话Clip


    public void SetPlayableDirector(PlayableDirector director)
    {
        currentPlayableDirector = director;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 按下按键，继续播放
            ResumeTimeline();
        }
    }

    public void PauseTimeline() //暂停播放
    {
        if (timelineMode == TimelineMode.Play)
        {
            currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);
            timelineMode = TimelineMode.Pause;

        }
    }

    public void ResumeTimeline() //继续播放
    {
        if (timelineMode == TimelineMode.Pause)
        {
            currentPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);
            timelineMode = TimelineMode.Play;
            if (isLastClip)
            {
                // 如果是最后一个对话Clip，继续播放时关闭对话窗口
                DialogueManager.instance.CloseDialogue();
            }
        }
    }
}
