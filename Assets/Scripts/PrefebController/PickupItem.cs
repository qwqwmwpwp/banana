using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public class PickupItem : MonoBehaviour
{
    public string name;
    public Sprite sprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<PlayerController>() != null)
        {
            PopupManager.instance.ShowItemTips("拾取" + name + "x1", "灯火森林独有的果实", "（技能树暂未开放，敬请期待）", null, false);
            Destroy(gameObject);
        }
    }
}
