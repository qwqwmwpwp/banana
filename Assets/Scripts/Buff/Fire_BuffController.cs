using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire_BuffController : MonoBehaviour
{
    public FireType fireType;
    public string fireName;
    
    private Animator anim;

    public void initialize(FireType fireType)
    {
        Debug.Log("initialize");

        this.fireType = fireType;
        anim = GetComponent<Animator>();


        if (this.fireType == FireType.火种余烛)
            fireName = "火种余辉";
        if (this.fireType == FireType.焦燃余烛)
        {
            fireName = "燃灼余辉";
            anim.SetBool("fire1", true);
        }
            
        if (this.fireType == FireType.锋芒余烛)
        {
            fireName = "锋芒余辉";
            anim.SetBool("fire2", true);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {

        if (!other.CompareTag("Player")) return; // 只对玩家响应

        BuffManager.instance.AddFire(fireType); // 拾取火焰



        // 延迟销毁
        Destroy(gameObject);
    }
}
