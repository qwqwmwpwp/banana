using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    bool x=true;
    public static SceneLoaderManager instance;


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
        Screen.SetResolution(2560, 1440, FullScreenMode.FullScreenWindow);
        Application.targetFrameRate = 60;
    }

    private void Update()
    {
        // 按下 F11 切换全屏 / 窗口
        if (Input.GetKeyDown(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        if(SceneManager.GetActiveScene().name == "火尾王关" && x)
        {
            BuffManager.instance.fireLeveStages = 1;
            x = false;
        }

    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // 等待加载完再激活

        while (!op.isDone)
        {
            Debug.Log($"加载进度: {op.progress * 100f}%");

            // 当进度到达90%时，正式进入
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
