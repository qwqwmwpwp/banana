using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public enum TriggerCondition
{
    区域进入触发,
    事件调用触发
}
public enum SpawnMode
{
    按时间刷新,
    按击杀刷新
}
public class EnemySpawner : MonoBehaviour
{

    [Header("触发模式")]
    [SerializeField] private TriggerCondition triggerCondition;

    [Header("刷新模式")]
    [SerializeField] private SpawnMode spawnMode;

    [Header("开启击杀切换所需击杀数")]
    [SerializeField] private int requiredKills;

    [System.Serializable]
    public class SpawnPointPair
    {
        public Transform point;          // 刷新点位置
        public EnemyTypes enemyType;     // 刷新该点的敌人类型
    }

    [Header("刷新点与敌人类型映射")]
    public List<SpawnPointPair> spawnPointPairs = new List<SpawnPointPair>();

    // 运行时字典：刷新点 → 敌人类型
    private Dictionary<Transform, EnemyTypes> spawnDict = new Dictionary<Transform, EnemyTypes>();

    [Header("敌人上限")]
    [SerializeField] private int enemyLimit;

    [Header("刷新波数与间隔")]
    [SerializeField] private int spawnWaveCount;
    [SerializeField] private float spawnInterval;



    private bool isSpawning = false;       // 是否正在刷怪
    private Coroutine spawnCoroutine;      // 刷怪协程
    private int aliveCount = 0;            // 当前刷怪器管理的存活敌人
    private int killCount = 0;             // 当前波次的击杀计数（击杀刷新模式用）

    private void Start()
    {
        // 循环设置字典
        foreach (var pair in spawnPointPairs)
        {
            if (pair.point != null)
            {
                if (!spawnDict.ContainsKey(pair.point))
                    spawnDict.Add(pair.point, pair.enemyType);
            }
        }
    }

    // 区域触发(只触发一次)
    private bool hasTriggered = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("区域进入触发1");
        if (triggerCondition == TriggerCondition.区域进入触发 && collision.GetComponent<PlayerController>()!=null)
        {
            if (hasTriggered) return;
            if (triggerCondition != TriggerCondition.区域进入触发) return;
            if (collision.GetComponent<PlayerController>() == null) return;

            hasTriggered = true;
            StartSpawner();
            Debug.Log("区域进入触发");
        }
    }

    #region 对外接口 

    // 开启刷怪器
    public void StartSpawner()
    {
        if (isSpawning) return;
        isSpawning = true;

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    //关闭刷怪器
    public void StopSpawner()
    {
        if (!isSpawning) return;
        isSpawning = false;

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = null;
    }

    // 敌人死亡时调用
    public void NotifyEnemyReturned()
    {
        // 刷新敌人存活数，用于判断是否达到最大值
        aliveCount = Mathf.Max(0, aliveCount - 1);

    }

    #endregion


    #region 核心刷怪逻辑 

    private IEnumerator SpawnRoutine()
    {
        for(int wave = 0; wave < spawnWaveCount; wave++)
        {
            foreach(var kv in spawnDict)
            {
                // 如果此时刷器关闭
                if (!isSpawning) yield break;

                Transform point = kv.Key;
                EnemyTypes type = kv.Value;

                // 达到上限 → 等待直到数量下降
                while (aliveCount >= enemyLimit)
                    yield return null;

                // 生成敌人
                GameObject enemy = EnemySpawnManager.instance.SpawnEnemy(type, point.position);
                if (enemy != null)
                    aliveCount++;

                yield return null; // 避免同帧刷多个
            }

            if (spawnMode == SpawnMode.按时间刷新)
            {
                yield return TimeWait();
            }
            else if (spawnMode == SpawnMode.按击杀刷新)
            {
                yield return KillWait();
            }

            Debug.Log("第" + wave + "波");

            // 重置击杀计数（仅击杀模式）
            EnemySpawnManager.instance.SetZeroEnemyDeathCount();


        }
        Debug.Log("所有波次刷完");
        isSpawning = false;

    }

    /// <summary>
    /// 按时间刷新：等待 spawnInterval 秒
    /// </summary>
    private IEnumerator TimeWait()
    {
        float t = 0;
        while (t < spawnInterval)
        {
            if (!isSpawning) yield break;

            t += Time.deltaTime;
            yield return null;
        }
    }

    /// <summary>
    /// 按击杀刷新：等待玩家击杀 requiredKills 个敌人
    /// </summary>
    private IEnumerator KillWait()
    {
        while (EnemySpawnManager.instance.currentLevelEnemyDeathCount < requiredKills)
        {
            if (!isSpawning) yield break;
            yield return null;
        }
    }

    #endregion 


}
