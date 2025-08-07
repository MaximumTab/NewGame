using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] private SpawningPattern Spawn;
    private List<TravelPoints> Routes;

    private void Start()
    {
        Incursion SpawnPoint = gameObject.GetComponent<Incursion>();
        SpawnPoint.GeneratePath();
        Routes = SpawnPoint.Routes;
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        float Timer = 0;
        int Iter = 0;
        while (Spawn.EnemySpawns.Last().SpawnTimer > Timer)
        {
            Timer += Time.fixedDeltaTime;
            if (Spawn.EnemySpawns[Iter].SpawnTimer < Timer)
            {
                GameObject Enemy= Instantiate(Spawn.EnemySpawns[Iter].Enemy,transform.position+Vector3.up*0.2f,Quaternion.identity);
                EnemyBehaviour CurEnem= Enemy.GetComponent<EnemyBehaviour>();
                CurEnem.Route = new TravelPoints();
                CurEnem.Route.CheckPoints = new List<Paths>();
                foreach (Paths path in Routes[Spawn.EnemySpawns[Iter].RouteTaken].CheckPoints)
                {
                    CurEnem.Route.CheckPoints.Add(new Paths(path.Objective));
                    CurEnem.Route.CheckPoints.Last().Path = new List<Vector3>();
                    foreach (Vector3 Location in path.Path)
                    {
                        CurEnem.Route.CheckPoints.Last().Path.Add(Location);
                    }
                }
                CurEnem.OnSpawn();
                CurEnem.SpawnSuccess = true;
                Iter++;
            }

            yield return null;
        }
    }
}

