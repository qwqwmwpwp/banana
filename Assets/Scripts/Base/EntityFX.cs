using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    private SpriteRenderer sr;
    private PlayerController player;

    [Header("材质")]
    [SerializeField] private float flahDuration;//闪光时间
    [SerializeField] private Material hitMat;//受击材质
    private Material originalMat;//原始材质

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalMat = sr.material;
        player = PlayerManager.instance.player;
    }

    public IEnumerator FlahFX()//用于显示受击的协程
    {
        sr.material = hitMat;//将材质修改为 hitMat

        Color currentColor = sr.color;
        sr.color = Color.white;

        yield return new WaitForSeconds(flahDuration);//等待for秒

        sr.color = currentColor;//这样整个协程不仅改变材质，还通过颜色变化增加了一个明显的闪烁效果。

        sr.material = originalMat;//恢复原始材质
    }
}
