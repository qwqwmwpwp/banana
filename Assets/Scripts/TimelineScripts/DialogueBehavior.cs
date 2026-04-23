using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// 对话行为控制器
[System.Serializable]
public class DialogueBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;

    [TextArea(1, 3)]
    [SerializeField] private string dialogueLines;
    [SerializeField] private string charactorName;
    [SerializeField] private Sprite ImageForNpc; //npc图像

    private bool isClipPlayed;    //标志位，保证只触发一次 SetDialogueToTimeline
    public bool requiredPause = true;  // 用户配置：是否需要在对话结束后暂停 Timeline
    public bool isLastClip = false;       // 用户配置：是否是动画中最后一个对话Clip

    /// <summary>
    /// 这是 PlayableBehaviour 类中的一个虚函数（virtual），当该 Playable 实例被创建时调用。
    ///
    ///它可以用来做初始化工作，比如获取组件引用、准备数据等。
    ///
    ///参数 Playable playable 表示当前被创建的可播放实例。
    /// </summary>
    public override void OnPlayableCreate(Playable playable)
    {
        //在 Playable 创建时获取当前 Timeline 的控制器引用
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;

        //传入 TimelineManager 中的 currentPlayableDirector 
        if (Application.isPlaying && TimelineManager.instance != null)
        {
            TimelineManager.instance.SetPlayableDirector(playableDirector);
        }
    }

    /// <summary>
    /// 是 PlayableBehaviour 类中的一个虚方法，Timeline 每帧播放时都会调用它，在这里实现实时更新的逻辑。
    /// </summary>
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        // 避免在编辑器直接拖动时间轴时调用导致报错
        if (!Application.isPlaying) return;

        //使用 info.weight 检测 clip 是否有效（weight > 0 说明当前 clip 正在 Blend 或播放）
        if (!isClipPlayed && info.weight > 0)
        {
            DialogueManager.instance.SetDialogueToTimeline(dialogueLines, charactorName, ImageForNpc, true);
            isClipPlayed = true;
        }

    }

    /// <summary>
    /// 这个方法会在以下两种情况下被调用：
    ///当前 Playable 暂停 时（例如 Timeline 被手动暂停）
    ///当前 Playable 不再活跃 时（例如时间片播放完毕、播放头离开该片段）
    /// </summary>
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        double currentTime = playable.GetTime();
        double duration = playable.GetDuration();

        // 只有 clip 播放时间接近结束时才执行暂停
        if (requiredPause && currentTime >= duration - 0.01f ) // 加个容错
        {
            TimelineManager.instance.PauseTimeline();

            //在这里传入 isLastClip，避免因为多个 Timeline Clip 共用同一个 DialogueBehavior 实例导致被最后一个clip覆盖
            TimelineManager.instance.isLastClip = isLastClip; 
        }
        else
        {
            // 若不暂停，可选择自动关闭对话等
            DialogueManager.instance.CloseDialogue();
        }

        // 为下一次 clip 播放做准备
        isClipPlayed = false;
    }
}
