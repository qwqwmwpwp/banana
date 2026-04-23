using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class StatrSceneController : MonoBehaviour
{
    [Header("UI组件")]
    public Button startButton;
    public Button exitButton;
    public Button optionsButton;

    [Header("黑屏设置")]
    public Image fadeImage;         // 用于黑屏的UI Image
    public float fadeDuration = 1f; // 渐变时间

    private void Start()
    {
        startButton?.onClick.AddListener(OnStartButtonClick);
        exitButton?.onClick.AddListener(OnExitGameClick);

    }


    public void OnStartButtonClick()
    {

        SceneLoaderManager.instance.LoadScene("Level1-0");
    }
    public void OnExitGameClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

   
}
