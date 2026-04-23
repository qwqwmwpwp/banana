using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCommon {

public class EnemySpawnContext
{
    public EnemyId enemyId;
    public GameObject gameObject;
    public List<Transform> patrolPoints;
    public Transform spawnPoint;
    

    public EnemySpawnContext(EnemyId enemyId, GameObject gameObject, List<Transform> patrolPoints = null, Transform spawnPoint = null)
    {
        this.enemyId = enemyId;
        this.gameObject = gameObject;
        this.patrolPoints = patrolPoints;
        this.spawnPoint = spawnPoint;  
    }
}

public enum SpawnModeOld
{
    Loop,
    Wave,
    SlaveLoop,    
}

[System.Serializable]
public class EnemySpawnInfo
{
    public EnemyId enemyId;
    public int count;
}

public enum EnemyId
{
    近战骚扰 = 1,
    高速近战 = 2,
    自爆 = 3,
    远程 = 4,
    FireElite = 5,
}

}