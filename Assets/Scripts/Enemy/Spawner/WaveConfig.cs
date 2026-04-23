using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnemyCommon
{
[CreateAssetMenu(menuName = "Enemy/Wave Config")]
public class WaveConfig : ScriptableObject
{
    public List<EnemySpawnInfo> enemies;
    public float spawnInterval = 0.5f;
    public float waveDelay = 3f;
}

}

