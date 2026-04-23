using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace EnemyCommon {

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    // 這個寫法可以避免 Inspector 不顯示 Dictionary 的問題
    [System.Serializable]
    public class EnemyPrefabEntry
    {
        public EnemyId enemyId;
        public GameObject prefab;
    }

    [Header("敵人 Prefab 列表")]
    [Tooltip("請在此註冊所有敵人的 Prefab 對應關係")]
    public List<EnemyPrefabEntry> enemyPrefabs;

    private Dictionary<EnemyId, GameObject> prefabDict;
    private Dictionary<EnemyId, ObjectPool<GameObject>> poolDict;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePrefabDictionary();
        InitializePools();
    }

    private void InitializePrefabDictionary()
    {
        prefabDict = new Dictionary<EnemyId, GameObject>();

        foreach (var entry in enemyPrefabs)
        {
            if (!prefabDict.ContainsKey(entry.enemyId) && entry.prefab != null)
            {
                prefabDict.Add(entry.enemyId, entry.prefab);
            }
            else
            {
                Debug.LogWarning($"[EnemyManager] 重複或空的 prefab 註冊：{entry.enemyId}");
            }
        }
    }

    private void InitializePools()
    {
        poolDict = new Dictionary<EnemyId, ObjectPool<GameObject>>();

        foreach (var kvp in prefabDict)
        {
            var prefab = kvp.Value;
            EnemyId id = kvp.Key;
            var enemy = prefab.GetComponent<EnemyBase>();

            var pool = new ObjectPool<GameObject>(
                () => Instantiate(prefab, transform),              // [1] createFunc: 當池中沒有可用物件時，如何創建新物件
                obj => obj.SetActive(true),             // [2] actionOnGet: 當從池中取出物件時要做什麼 (這裡讓物件啟用)
                obj => obj.SetActive(false),            // [3] actionOnRelease: 當物件被釋放回池中時的處理 (這裡讓它關閉)
                obj => Destroy(obj),                    // [4] actionOnDestroy: 當池子超過上限或被清空時，如何銷毀物件
                true,                                   // [5] collectionCheck: 是否啟用重複回收檢查 (true 則會報錯)
                20,                                     // [6] defaultCapacity: 初始建立的預設物件數量 (可為 0)
                100                                     // [7] maxSize: 物件池的最大容量 (超過會直接銷毀)
            );

            poolDict.Add(id, pool);
        }
    }

    public GameObject GetFromPool(EnemyId enemyId)
    {
        if (poolDict.TryGetValue(enemyId, out var pool))
        {
            return pool.Get();
        }

        Debug.LogError($"[EnemyManager] 查無對應 pool : {enemyId}");
        return null;
    }

    public void ReturnToPool(EnemyId enemyId, GameObject obj)
    {
        if (poolDict.TryGetValue(enemyId, out var pool))
        {
            pool.Release(obj);
        }
        else
        {
            Debug.LogWarning($"[EnemyManager] 無法回收，直接銷毀 : {enemyId}");
            Destroy(obj);
        }
    }
}

}
