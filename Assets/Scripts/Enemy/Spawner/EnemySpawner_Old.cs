using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCommon {

public partial class EnemySpawner_Old : MonoBehaviour
{
    // 刷新模式
    public SpawnModeOld spawnMode = SpawnModeOld.Loop;

    // 刷新設定
    public List<Transform> spawnPoints;
    public List<Transform> patrolPoints;

    // 波次模式設定
    public List<WaveConfig> waves;
    public float delayBetweenWaves = 5f;
    private int currentWave = 0;

    // 循環模式設定 
 
    public EnemyId loopEnemyId;
    public float loopSpawnInterval = 2f;
    public int loopSpawnCount;

    // 主從循環模式設定
    public EnemySpawnerMaster esm;
    
    // 通用參數
    public int maxAliveEnemies = 10;
    private int aliveEnemyCount = 0;
    [SerializeField]
    private bool isSpawning = false;

    public void Start()
    {
        switch (spawnMode)
        {
            case SpawnModeOld.Loop:
                StartCoroutine(LoopSpawn());
                break;
            case SpawnModeOld.Wave:
                StartCoroutine(SpawnWaves());
                break;
            case SpawnModeOld.SlaveLoop:
                StartCoroutine(SlaveLoopSpawn());
                break;
        }
    }

#region 波次刷新
    public virtual IEnumerator SpawnWaves()
    {
        yield return new WaitForSeconds(2f); // First wave delay

        while (currentWave < waves.Count)
        {
            if (!isSpawning && aliveEnemyCount <= 0)
            {
                yield return StartCoroutine(SpawnWave(waves[currentWave]));
                currentWave++;
                yield return new WaitForSeconds(delayBetweenWaves);
            }
            else
            {
                yield return new WaitForSeconds(delayBetweenWaves); // 等待場上敵人清空
            }
        }
    }

    public virtual IEnumerator SpawnWave(WaveConfig waveConfig)
    {
        isSpawning = true;

        foreach (var enemyInfo in waveConfig.enemies)
        {
            for (int i = 0; i < enemyInfo.count; i++)
            {
                if (aliveEnemyCount >= maxAliveEnemies)
                {
                    isSpawning = false;
                    yield break;
                }

                SpawnEnemy(enemyInfo.enemyId);
                yield return new WaitForSeconds(waveConfig.spawnInterval);
            }
        }

        isSpawning = false;
    }
#endregion

#region 循環刷新
    private IEnumerator LoopSpawn()
    {
        yield return new WaitForSeconds(loopSpawnInterval); // 初始延遲

        while (true)
        {
            if (!isSpawning && aliveEnemyCount < maxAliveEnemies)
            {
                isSpawning = true;

                for (int i = 0; i < loopSpawnCount; ++i) SpawnEnemy(loopEnemyId);

                yield return new WaitForSeconds(loopSpawnInterval);

                isSpawning = false;
            }
            else
            {
                yield return null;
            }
        }
    }
#endregion

#region 主從循環刷新
    private IEnumerator SlaveLoopSpawn()
    {
        yield return new WaitForSeconds(loopSpawnInterval); // 初始延遲    

        while (true)
        {
            if (!isSpawning && esm != null && esm.CanSpawn())
            {
                isSpawning = true;  

                SpawnEnemy(loopEnemyId);    

                yield return new WaitForSeconds(loopSpawnInterval); 

                isSpawning = false;
            }
            else
            {
                yield return null;
            }
        }
    }   
#endregion

#region 通用函數
    public virtual void SpawnEnemy(EnemyId enemyId)
    {
        if (aliveEnemyCount >= maxAliveEnemies) return;

        GameObject enemyObject = EnemyManager.Instance.GetFromPool(enemyId);
        if (enemyObject == null)
        {
            Debug.LogWarning($"[Spawner] 無法找到敵人 Prefab: {enemyId}");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        enemyObject.transform.position = spawnPoint.position;

        var ctx = new EnemySpawnContext(enemyId, enemyObject, patrolPoints, spawnPoint);

        // 這邊使用事件都需要先做防呆處理 不然一定會報錯

        // 假設敵人死亡時會呼叫這個 Spawner 的方法
        var stats = enemyObject.GetComponent<EnemyStats>();
        stats.ctx = ctx;
        // 確保重複物件取出時不會重複加載兩次事件
        stats.OnDeath -= OnEnemyKilled;
        stats.OnDeath += OnEnemyKilled;
        stats.Reset();

        var enemy = enemyObject.GetComponent<EnemyBase>();
        enemy.Init(ctx);

        aliveEnemyCount++;
        esm?.OnEnemySpawned();
    }

    public virtual void OnEnemyKilled(EnemySpawnContext ctx)
    {
        aliveEnemyCount = Mathf.Max(0, aliveEnemyCount - 1);
        EnemyManager.Instance.ReturnToPool(ctx.enemyId, ctx.gameObject);
        esm?.OnEnemyKilled();
    }
#endregion
}

}
