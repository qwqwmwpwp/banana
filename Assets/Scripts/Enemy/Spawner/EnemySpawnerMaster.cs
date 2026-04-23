using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCommon
{

public class EnemySpawnerMaster : MonoBehaviour
{
    public int globalMaxAliveEnemies = 30;

    private int totalAliveEnemies = 0;

    public bool CanSpawn()
    {
        return totalAliveEnemies < globalMaxAliveEnemies;
    }

    public void OnEnemySpawned()
    {
        totalAliveEnemies++;
    }

    public void OnEnemyKilled()
    {
        totalAliveEnemies = Mathf.Max(0, totalAliveEnemies - 1);
    }

    public int GetAliveCount()
    {
        return totalAliveEnemies;
    }
}

}