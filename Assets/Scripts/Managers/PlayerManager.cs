using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    //设置 PlayerManager 单例，用于后续统一管理

    public static PlayerManager instance;

    public PlayerController player;

    [Header("复活点")]
    public Transform respawnPoint;  // 复活点

    public int deathNumber; // 当前关卡死亡次数

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
    }

    private void Update()
    {
        UnlockNoteMechanism();
        if (Input.GetKeyUp(KeyCode.B))
        {
            player.stats.Die();
        }
    }

    // 根据当前关卡与死亡次数解锁提示
    private void UnlockNoteMechanism()
    {
        if (SceneManager.GetActiveScene().name == "火开门关")
        {
            if (deathNumber == 1)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(0);
            if (deathNumber == 4)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(1);
        }
        if (SceneManager.GetActiveScene().name == "火老一关")
        {
            if (deathNumber == 1)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(2);
            if (deathNumber == 5)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(3);
        }
        if (SceneManager.GetActiveScene().name == "火祭坛关")
        {
            if (deathNumber == 3)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(4);
            if (deathNumber == 5)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(5);
            if (deathNumber == 8)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(6);
        }
        if (SceneManager.GetActiveScene().name == "火尾王关")
        {
            if (deathNumber == 3)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(7);
            if (deathNumber == 5)
                UIManager.instance.Note.mechanism.GetComponent<UI_Mechanism>().UnlockTips(8);
        }
    }
}
