using DG.Tweening;
using Spine;
using Spine.Unity;

/// <summary>
/// 自己定义的 spine 的 DOTween 拓展方法(工具类)
/// </summary>
public static class DOTweenSpineExtensions
{
    
    public static Tweener DOColor(this Skeleton skeleton, UnityEngine.Color endValue, float duration)
    {
        // 记录初始颜色
        UnityEngine.Color startColor = skeleton.GetColor();

        /// <summary>
        /// DOTween.To(getter, setter, endValue, duration) ：DOTween 的核心方法，对某个数值属性进行插值补间动画（位置、缩放、音量、颜色等）
        /// getter ：当前值的获取方式，在补间过程中，DOTween 会不断调用它来获取最新值。
        /// setter ：接收 DOTween 计算出的新值，并赋给目标
        /// endValue ：动画最终会到达的值。
        /// duration ：动画从当前值到目标值花费的时间s
        /// <summary>
        Tweener tweener = DOTween.To(
            () => skeleton.GetColor(),
            x => skeleton.SetColor(x),
            endValue,
            duration
        );

        return tweener;
    }
   

}
