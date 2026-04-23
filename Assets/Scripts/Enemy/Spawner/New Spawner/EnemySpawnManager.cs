using System.Collections.Generic;
using UnityEngine;

public enum EnemyTypes
{
    远程,
    自爆,
    近战骚扰,
    近战高速
}

public class EnemySpawnManager : MonoBehaviour
{
    #region 单例
    public static EnemySpawnManager instance;
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
    #endregion

    // 当前计入的敌人死亡数量
    public int currentLevelEnemyDeathCount { get; private set; }

    // 可序列化的敌人类型-预制体对
    [System.Serializable]
    public class EnemyPair
    {
        public EnemyTypes type;      
        public GameObject prefab;
        public int poolSize = 10;        // 池子初始大小：预先创建的对象数量
    }

    // 利用 List 在外部显示，游戏开始时通过 Start 中的 foreach 循环设置字典
    public List<EnemyPair> enemyList = new List<EnemyPair>();
    public Dictionary<EnemyTypes, GameObject> Enemys = new Dictionary<EnemyTypes, GameObject>();

    // 对象池：类型 -> 对象队列
    private Dictionary<EnemyTypes, Queue<GameObject>> enemyPools = new Dictionary<EnemyTypes, Queue<GameObject>>();

    private void Start()
    {
        SetZeroEnemyDeathCount();

        foreach (var e in enemyList)
        {
            if (!Enemys.ContainsKey(e.type))
            {
                Enemys.Add(e.type, e.prefab);

                // 创建对应怪物池，避免大量 Instantiate / Destroy 解决内存分配、回收造成的性能抖动
                Queue<GameObject> pool = new Queue<GameObject>();
                for (int i = 0; i < e.poolSize; i++)
                { 
                    // 创建对象
                    GameObject obj = Instantiate(e.prefab);
                    obj.GetComponent<EnemyBase>().Init(SpawnType.刷怪器生成);
                    // 不激活
                    obj.SetActive(false);
                    // 加入队列
                    pool.Enqueue(obj);
                }

                enemyPools.Add(e.type, pool);
            }
        }
    }

    // 生成敌人的方法
    public GameObject SpawnEnemy(EnemyTypes type, Vector3 pos)
    {
        if (!enemyPools.ContainsKey(type))
        {
            Debug.LogError("没有找到这个敌人的池子：" + type);
            return null;
        }

        GameObject enemy;

        // 如果池子有对象
        if (enemyPools[type].Count > 0)
        {
            // 取出队头元素
            enemy = enemyPools[type].Dequeue();
        }
        else
        {
            // 如果池子没对象了，动态生成一个（可选策略）
            enemy = Instantiate(Enemys[type]);
        }

        // 设置位置并激活
        enemy.transform.position = pos;
        enemy.SetActive(true);

        return enemy;
    }

    // 敌人死亡返回对象池
    public void ReturnEnemy(EnemyTypes type, GameObject enemy)
    {
        // 关闭对象并从对尾入队
        enemy.SetActive(false);
        enemyPools[type].Enqueue(enemy);
        // 增加当前计入的敌人死亡数量
        currentLevelEnemyDeathCount++;
    }

    public void SetZeroEnemyDeathCount()
    {
        currentLevelEnemyDeathCount = 0;
    }

    // 清除场内所有敌人
    public void ClearAllEnemies()
    {
        // 找到当前场景中所有敌人
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();

        foreach (var e in enemies)
        {
            ReturnEnemy(e.type, e.gameObject);
        }
    }
}
