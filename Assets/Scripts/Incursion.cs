using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
public class Incursion : MonoBehaviour
{
    [Serializable]
     public struct TravelPoints
     {
         public List<Paths> CheckPoints;

     }
    [Serializable]
    public  class Paths
    {
        public Transform Objective;
        public Dictionary<Vector3, int> Distances;
        public List<Vector3> Path;

        public int Dist(Vector3 Key)
        {
            return Distances[Key];
        }

        public bool ContainsKey(Vector3 Key)
        {
            return Distances.ContainsKey(Key);
        }

        public int GetValue(Vector3 Key)
        {
            return Distances[Key];
        }

        public void SetValue(Vector3 Key, int Index)
        {
            Distances[Key] = Index;
        }

        public void EmpAndAddDist(Path[] Tiles)
        {
            Distances = new Dictionary<Vector3, int>();
            foreach (Path path in Tiles)
            {
                Distances.Add(path.transform.position,0);
            }
        }

        public void OnDrawGizmos()
        {
            for (int i = 0; i < Path.Count - 1; i++)
            {
                Gizmos.DrawLine(Path[i],Path[i+1]);
            }
        }

        public void AddTile(Vector3 Key)
        {
            if (!Path.Contains(Key))
            {
                Path.Add(Key);
            }
        }

        public void ReduceNodes()
        {
            Shortcut(2);
            List<Vector3> ResultPath=new List<Vector3>();
            Vector3 LastNode=Path[0];
            Vector3 PreviousTile=Path[0];
            ResultPath.Add(LastNode);
            foreach (Vector3 Tile in Path)
            {
                if (!Mathf.Approximately(LastNode.x, Tile.x) && !Mathf.Approximately(LastNode.z, Tile.z))
                {
                    ResultPath.Add(PreviousTile);
                    LastNode = PreviousTile;
                }

                PreviousTile = Tile;
            }
            ResultPath.Add(Path.Last());
            Path = ResultPath;
        }

        public void Shortcut(int Buffer)
        {
            List<Vector3> ResultPath=new List<Vector3>();
            ResultPath.Add(Path[0]);
            for (int i = Buffer; i < Path.Count; i++)
            {
                int Strikes = 2;
                foreach (Vector3 neighbour1 in UtilPath.AllAdjDirections(Path[i-Buffer]))
                {
                    foreach (Vector3 neighbour2 in UtilPath.AllAdjDirections(Path[i]))
                    {
                        if (neighbour1 == neighbour2 && Distances.ContainsKey(neighbour1))
                        {
                            Strikes--;
                        }
                    }
                }
                if (Strikes != 0)
                {
                    ResultPath.Add(Path[i-1]);
                }
            }
            ResultPath.Add(Path.Last());
            Path = ResultPath;
        }
    }
    public int ActivePath = 0;
    public bool CreatePath;
    public List<Vector3> Visited;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public TravelPoints[] Routes;
    private void Update() 
    {
        if (Routes.Length != 0)
        {
            if (CreatePath)
            {
                CreatePath = false;
                GeneratePath();
            }
        }
    }

    private void GeneratePath()
    {
        Path[] Tiles = transform.parent.GetComponentsInChildren<Path>();
        for (int i = 0; i < Routes[ActivePath].CheckPoints.Count; i++)
        {
            Visited = new List<Vector3>();
            Routes[ActivePath].CheckPoints[i].EmpAndAddDist(Tiles);

            AddValues(Routes[ActivePath].CheckPoints[i].Objective.position, 1, i);
        }
        MakePath();
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Routes[ActivePath].CheckPoints.Count; i++)
        {
            Routes[ActivePath].CheckPoints[i].OnDrawGizmos();
        }
    }

    private void AddValues(Vector3 StartLoc, int Index,int i)
    {
        Visited.Add(StartLoc);
        if (StartLoc != transform.position)
        {
            foreach (Vector3 Dir in UtilPath.AllAdjDirections(StartLoc))
            {
                if (Routes[ActivePath].CheckPoints[i].ContainsKey(Dir) && !Visited.Contains(Dir))
                {
                    Routes[ActivePath].CheckPoints[i].SetValue(Dir,Index);
                    AddValues(Dir,Index+1,i);
                }
                else if(Routes[ActivePath].CheckPoints[i].ContainsKey(Dir)&&Routes[ActivePath].CheckPoints[i].GetValue(Dir)>Index)
                {
                    Routes[ActivePath].CheckPoints[i].SetValue(Dir,Index);
                    AddValues(Dir,Index+1,i);
                }
            }
        }
    }

    private void MakePath()
    {
        Vector3 ShortRoute = transform.position;
        for(int i=0;i<Routes[ActivePath].CheckPoints.Count;i++)
        {
            Routes[ActivePath].CheckPoints[i].Path = new List<Vector3>();
            Routes[ActivePath].CheckPoints[i].AddTile(ShortRoute);
            Vector3 NextRoute = RightDir(ShortRoute,i);
            while (ShortRoute != NextRoute)
            {
                Routes[ActivePath].CheckPoints[i].AddTile(NextRoute);
                ShortRoute = NextRoute;
                NextRoute = RightDir(ShortRoute,i);
            }
            Routes[ActivePath].CheckPoints[i].ReduceNodes();
        }
    }
    private void MakePath(Vector3 Curloc,int CurCheckPoint)
    {
        Vector3 ShortRoute = Curloc;
        for(int i=CurCheckPoint;i<Routes[ActivePath].CheckPoints.Count;i++)
        {
            Vector3 NextRoute = RightDir(ShortRoute,i);
            while (ShortRoute != NextRoute)
            {
                Routes[ActivePath].CheckPoints[i].AddTile(ShortRoute);
                ShortRoute = NextRoute;
                NextRoute = RightDir(ShortRoute,i);
            }

        }
    }

    private Vector3 RightDir(Vector3 Loc,int i)
    {
        Vector3 RightOne=Loc;
        foreach (Vector3 Dir in UtilPath.AllAdjDirections(Loc))
        {
            if (Routes[ActivePath].CheckPoints[i].ContainsKey(Dir)&&Routes[ActivePath].CheckPoints[i].ContainsKey(RightOne))
            {
                if (Routes[ActivePath].CheckPoints[i].GetValue(Dir) < Routes[ActivePath].CheckPoints[i].GetValue(RightOne))
                {
                    RightOne = Dir;
                }
            }else if (Routes[ActivePath].CheckPoints[i].ContainsKey(Dir))
            {
                RightOne = Dir;
            }
        }

        return RightOne;
    }
}

public static class UtilPath
{
    public static Vector3[] AllAdjDirections(Vector3 Loc)
    {
        return new Vector3[]
        {
            Loc + Vector3.forward,
            Loc + Vector3.back,
            Loc + Vector3.left,
            Loc + Vector3.right
        };
    }
    public static Vector3[] AllDirections(Vector3 Loc)
    {
        return new Vector3[]
        {
            Loc + Vector3.forward,
            Loc + Vector3.back,
            Loc + Vector3.left,
            Loc + Vector3.right,
            Loc + Vector3.forward + Vector3.left,
            Loc + Vector3.back + Vector3.right,
            Loc + Vector3.left + Vector3.back,
            Loc + Vector3.right + Vector3.forward
        };
    }
}
