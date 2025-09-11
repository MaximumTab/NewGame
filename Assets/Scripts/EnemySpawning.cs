using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EnemySpawning : Incursion
{
    public List<EnemySpawn> EnemySpawns;
    private GameManager GM;
    [Serializable]
    public class EnemySpawn
    {
        public float SpawnTimer;
        public int RouteTaken;
        public GameObject Enemy;
        public List<TunnnelsAndTime> TunnelTimes;
        public EnemySpawn()
        {
            TunnelTimes = new List<TunnnelsAndTime>();
        }

        [Serializable]
        public class TunnnelsAndTime
        {
            public Vector3 Enter;
            public Vector3 Exit;
            public float Time;
        }
    }

    public int EnemyCount;

    public override void OnDrawGizmos()
    {
        try
        {
            foreach (EnemySpawn ES in EnemySpawns)
            {
                int TunnelIndex = 0;
                foreach (Paths paths in Routes[ES.RouteTaken].CheckPoints)
                {
                    for (int i = 0; i < paths.Path.Count; i++)
                    {
                        if (Tunnels.ContainsKey(paths.Path[i]) && paths.Path.Count > i + 1)
                        {
                            if (ES.TunnelTimes.Count == TunnelIndex)
                            {
                                EnemySpawn.TunnnelsAndTime TAT = new EnemySpawn.TunnnelsAndTime();
                                TAT.Enter = paths.Path[i];
                                TAT.Exit = paths.Path[i + 1];
                                ES.TunnelTimes.Add(TAT);
                            }

                            i++;
                            TunnelIndex++;
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            CreatePath = true;
        }
        base.OnDrawGizmos();
    }

    public List<EnemySpawn> GetSpawnOrdered()
    {
        EnemySpawns.Sort((s1, s2) => s1.SpawnTimer.CompareTo(s2.SpawnTimer));
        return EnemySpawns;
    }

    private void Start()
    {
        StartCoroutine(SpawnCycle());
        if (!GM)
        {
            EnemyCount = EnemySpawns.Count;
            EnemyCount-=EnemySpawns.FindAll(es => es.Enemy.GetComponent<TrailPath>()).Count;
            GM = FindFirstObjectByType<GameManager>();
            GM.SetEnemyCount(EnemyCount);
        }
    }
    

    IEnumerator SpawnCycle()
    {
        yield return null;
        GeneratePath();
        yield return new WaitForSeconds(0.2f);
        EnemySpawns = GetSpawnOrdered();
        float Timer = 0;
        int Iter = 0;
        while (EnemySpawns.Last().SpawnTimer > Timer)
        {
            Timer += Time.deltaTime;
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
                    CurEnem.Route.CheckPoints.Last().WaitSeconds = path.WaitSeconds;
                    foreach (Vector3 Location in path.Path)
                    {
                        CurEnem.Route.CheckPoints.Last().Path.Add(Location);
                    }
                }
                CurEnem.OnSpawn();
                CurEnem.TunnelLocs = Tunnels;
                CurEnem.TunnelTimes = new List<float>();
                CurEnem.ForLeak = true;
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
[CustomEditor(typeof(EnemySpawning))]
public class EnemSpawnEditor : Editor
{
    private int EnemyLength;
    private List<GameObject> EnemyList=new List<GameObject>();
    private List<int> InstanceAmt = new List<int>();
    public void CompFix<T>(int ImportantComp, List<T> ArrayChange, T DefValue)
    {
        if (ImportantComp < 1)
        {
            ImportantComp = 1;
        }

        if (ImportantComp < ArrayChange.Count)
        {
            ArrayChange.Remove(ArrayChange.Last());
        }
        else if(ImportantComp!=ArrayChange.Count)
        {
            ArrayChange.Add(DefValue);
        }
    }
    public void CompFix<T,A>(int ImportantComp, Dictionary<T,A> ArrayChange, T DefKey,A DefValue)
    {
        if (ImportantComp < 1)
        {
            ImportantComp = 1;
        }

        if (ImportantComp < ArrayChange.Count)
        {
            ArrayChange.Remove(ArrayChange.Keys.Last());
        }
        else if(ImportantComp!=ArrayChange.Count)
        {
            ArrayChange.Add(DefKey,DefValue);
        }
    }

    public override void OnInspectorGUI()
    {
        EnemySpawning thisESpawn = (EnemySpawning)target;
        EnemyList = new List<GameObject>();
        InstanceAmt = new List<int>();
        foreach (EnemySpawning.EnemySpawn ES in thisESpawn.EnemySpawns)
        {
            if (!EnemyList.Contains(ES.Enemy)&&ES.Enemy)
            {
                InstanceAmt.Add(thisESpawn.EnemySpawns.FindAll(es=>es.Enemy==ES.Enemy).Count);
                EnemyList.Add(ES.Enemy);
            }
        }

        EnemyLength = EnemyList.Count;

        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Routes");
        CompFix(EditorGUILayout.IntField("Amount of Routes", thisESpawn.Routes.Count),thisESpawn.Routes,new TravelPoints());
        foreach (TravelPoints TPoints in thisESpawn.Routes)
        {
            EditorGUILayout.LabelField("Route "+thisESpawn.Routes.IndexOf(TPoints));
            CompFix(EditorGUILayout.IntField("Route "+thisESpawn.Routes.IndexOf(TPoints)+" CheckPoints", TPoints.CheckPoints.Count),TPoints.CheckPoints,new Paths(thisESpawn.transform));
            foreach (Paths paths in TPoints.CheckPoints)
            {
                paths.Objective =(Transform)EditorGUILayout.ObjectField("CheckPoint "+TPoints.CheckPoints.IndexOf(paths), paths.Objective,paths.Objective.GetType());
                paths.WaitSeconds = EditorGUILayout.FloatField("Waiting for", paths.WaitSeconds);
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
        EditorGUILayout.BeginFoldoutHeaderGroup(true, "Enemies");
        EnemyLength = EditorGUILayout.IntField("Unique Enemy Count", EnemyLength);
        CompFix(EnemyLength,EnemyList,thisESpawn.gameObject);
        CompFix(EnemyLength,InstanceAmt,1);
        int temp = 0;
        for (int i = 0; i < EnemyList.Count; i++)
        {
            EnemyList[i] =(GameObject) EditorGUILayout.ObjectField(EnemyList[i].name, EnemyList[i], EnemyList[i].GetType());
            InstanceAmt[i]= EditorGUILayout.IntField(EnemyList[i].name + "Amount", InstanceAmt[i]);
            EditorGUILayout.Foldout(true, EnemyList[i].name + " Instances",true);
            for (int k = 0; k < InstanceAmt[i]; k++)
            {
                if (thisESpawn.EnemySpawns.Count <= temp)
                {
                    EnemySpawning.EnemySpawn tempESpawn = new EnemySpawning.EnemySpawn
                    {
                        Enemy = EnemyList[i]
                    };
                    thisESpawn.EnemySpawns.Add(tempESpawn);
                }
                thisESpawn.EnemySpawns[temp].SpawnTimer=EditorGUILayout.FloatField("Instance "+k+" Spawn timer",thisESpawn.EnemySpawns[temp].SpawnTimer);
                thisESpawn.EnemySpawns[temp].RouteTaken=EditorGUILayout.IntField("Instance "+k+" Route taken",thisESpawn.EnemySpawns[temp].RouteTaken);
                for (int j = 0;j<thisESpawn.EnemySpawns[temp].TunnelTimes.Count; j++)
                {
                    thisESpawn.EnemySpawns[temp].TunnelTimes[j].Time=EditorGUILayout.FloatField("Tunneling "+j+" for",thisESpawn.EnemySpawns[temp].TunnelTimes[j].Time);
                }

                temp++;
            }
        }

        CompFix(temp, thisESpawn.EnemySpawns, new EnemySpawning.EnemySpawn());
        thisESpawn.EnemyCount = thisESpawn.EnemySpawns.Count;
        thisESpawn.EnemyCount-=thisESpawn.EnemySpawns.FindAll(es => es.Enemy.GetComponent<TrailPath>()).Count;

        EditorGUILayout.EndFoldoutHeaderGroup();
        serializedObject.ApplyModifiedProperties();
    }
}

