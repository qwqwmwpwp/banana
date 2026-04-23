using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// 对话片段容器

public class DialogueClip : PlayableAsset
{
    public DialogueBehavior template = new DialogueBehavior();

    /// <summary>
    /// 在 Timeline 播放到某个 Clip 时，生成一个附带自定义行为（DialogueBehavior）的 Playable 实例，并将它交由 PlayableGraph 进行调度和播放。
    /// 
    /// CreatePlayable 是 PlayableAsset 类的一个虚函数，表示当 Timeline 播放该片段（Clip）时，它会调用这个方法来生成实际运行时使用的 Playable
    /// 
    /// PlayableGraph graph 是 Unity 提供的播放图（一个有向图结构），它管理所有的 Playable 实例及其连接关系。
    ///
    /// GameObject owner 是拥有此 PlayableAsset 的游戏对象（通常是绑定在 Timeline Track 上的对象）。
    /// </summary>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        //在这个播放图里新建一个节点，节点的行为逻辑是我自己写的 DialogueBehavior
        var playable = ScriptPlayable<DialogueBehavior>.Create(graph, template);
        return playable;
    }
}
