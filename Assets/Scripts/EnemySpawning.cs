using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    private List<TravelPoints> Routes;
    private Dictionary<Vector3,Tunnel> TunnelLocs;
    public List<EnemySpawn> EnemySpawns;
    [Serializable]
    public struct EnemySpawn
    {
        public float SpawnTimer;
        public int RouteTaken;
        public GameObject Enemy;
        public List<TunnnelsAndTime> TunnelTimes;
        [Serializable]
        public struct TunnnelsAndTime
        {
            public Vector3 Enter;
            public Vector3 Exit;
            public float Time;
        }
    }


    private void OnDrawGizmos()
    {
        Incursion SpawnPoint = gameObject.GetComponent<Incursion>();
        Routes = SpawnPoint.Routes;
        TunnelLocs = SpawnPoint.Tunnels;
        foreach (EnemySpawn ES in EnemySpawns)
        {
            int TunnelIndex = 0;
            foreach (Paths paths in Routes[ES.RouteTaken].CheckPoints)
            {
                for (int i=0;i<paths.Path.Count;i++)
                {
                    if (TunnelLocs.ContainsKey(paths.Path[i])&&paths.Path.Count>i+1)
                    {
                        if (ES.TunnelTimes.Count == TunnelIndex)
                        {
                            EnemySpawn.TunnnelsAndTime TAT = new EnemySpawn.TunnnelsAndTime();
                            TAT.Enter = paths.Path[i];
                            TAT.Exit = paths.Path[i+1];
                            ES.TunnelTimes.Add(TAT);
                        }

                        i++;
                        TunnelIndex++;
                    }
                }
            }
        }
    }

    public List<EnemySpawn> GetSpawnOrdered()
    {
        EnemySpawns.Sort((s1, s2) => s1.SpawnTimer.CompareTo(s2.SpawnTimer));
        return EnemySpawns;
    }

    private void Start()
    {
        Incursion SpawnPoint = gameObject.GetComponent<Incursion>();
        SpawnPoint.GeneratePath();
        Routes = SpawnPoint.Routes;
        TunnelLocs = SpawnPoint.Tunnels;
        StartCoroutine(SpawnCycle());
    }

    IEnumerator SpawnCycle()
    {
        float Timer = 0;
        int Iter = 0;
        while (EnemySpawns.Last().SpawnTimer > Timer)
        {
            Timer += Time.fixedDeltaTime;
            if (EnemySpawns[Iter].SpawnTimer < Timer)
            {
                GameObject Enemy= Instantiate(EnemySpawns[Iter].Enemy,transform.position+Vector3.up*0.2f,Quaternion.identity);
                EnemyBehaviour CurEnem= Enemy.GetComponent<EnemyBehaviour>();
                CurEnem.Route = new TravelPoints();
                CurEnem.Route.CheckPoints = new List<Paths>();
                foreach (Paths path in Routes[EnemySpawns[Iter].RouteTaken].CheckPoints)
                {
                    CurEnem.Route.CheckPoints.Add(new Paths(path.Objective));
                    CurEnem.Route.CheckPoints.Last().Path = new List<Vector3>();
                    foreach (Vector3 Location in path.Path)
                    {
                        CurEnem.Route.CheckPoints.Last().Path.Add(Location);
                    }
                }
                CurEnem.OnSpawn();
                CurEnem.TunnelLocs = TunnelLocs;
                CurEnem.TunnelTimes = new List<float>();
                foreach (EnemySpawn.TunnnelsAndTime TAT in EnemySpawns[Iter].TunnelTimes)
                {
                    CurEnem.TunnelTimes.Add(TAT.Time);
                }
                CurEnem.SpawnSuccess = true;
                Iter++;
            }

            yield return null;
        }
    }
}

