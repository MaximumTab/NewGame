using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawningPattern", menuName = "Spawning/SpawningPattern")]
public class SpawningPattern : ScriptableObject
{
    public List<EnemySpawn> EnemySpawns;
    [Serializable]
    public struct EnemySpawn
    {
        public float SpawnTimer;
        public int RouteTaken;
        public GameObject Enemy;
    }

    

    public List<EnemySpawn> GetSpawnOrdered()
    {
        EnemySpawns.Sort((s1, s2) => s1.SpawnTimer.CompareTo(s2.SpawnTimer));
        return EnemySpawns;
    }
}
