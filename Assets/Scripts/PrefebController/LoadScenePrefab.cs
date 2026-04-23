using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class LoadScenePrefab : MonoBehaviour
{
    public string SceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 检查进入的是玩家
        if (collision.GetComponent<PlayerController>() != null)
        {
            SceneLoaderManager.instance.LoadScene(SceneName);
        }
    }
}
