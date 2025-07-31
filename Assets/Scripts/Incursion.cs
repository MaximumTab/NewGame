using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[ExecuteInEditMode]
public class Incursion:MonoBehaviour
{
    
    public int ActivePath = 0;
    public bool CreatePath;
    public List<Vector3> Visited;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public TravelPoints[] Routes;

    public void GeneratePath()
    {
        for (int k = 0; k < Routes.Length; k++)
        {
            Path[] Tiles = transform.parent.GetComponentsInChildren<Path>();
            for (int i = 0; i < Routes[k].CheckPoints.Count; i++)
            {
                Visited = new List<Vector3>();
                Routes[k].CheckPoints[i].EmpAndAddDist(Tiles);

                AddValues(Routes[k].CheckPoints[i].Objective.position, 1, i,k);
                Routes[k].CheckPoints[i].MakePath(i==0?transform.position:Routes[k].CheckPoints[i-1].Objective.transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Routes.Length != 0)
        {
            if (CreatePath)
            {
                CreatePath = false;
                GeneratePath();
            }
        }
        for (int i = 0; i < Routes[ActivePath].CheckPoints.Count; i++)
        {
            Routes[ActivePath].CheckPoints[i].OnDrawGizmos();
        }
    }

    private void AddValues(Vector3 StartLoc, int Index,int i,int RouteID)
    {
        Visited.Add(StartLoc);
        if (StartLoc != transform.position)
        {
            foreach (Vector3 Dir in UtilPath.AllAdjDirections(StartLoc))
            {
                if (Routes[RouteID].CheckPoints[i].ContainsKey(Dir) && !Visited.Contains(Dir))
                {
                    Routes[RouteID].CheckPoints[i].SetValue(Dir,Index);
                    AddValues(Dir,Index+1,i,RouteID);
                }
                else if(Routes[RouteID].CheckPoints[i].ContainsKey(Dir)&&Routes[RouteID].CheckPoints[i].GetValue(Dir)>Index)
                {
                    Routes[RouteID].CheckPoints[i].SetValue(Dir,Index);
                    AddValues(Dir,Index+1,i,RouteID);
                }
            }
        }
    }
}
[Serializable]
public class Paths
{
    public Transform Objective;
    public Dictionary<Vector3, int> Distances;
    public List<Vector3> Path;
    public float WaitSeconds;

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
        Shortcut();
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

    public void Shortcut()
    {
        List<Vector3> ResultPath=new List<Vector3>();
        ResultPath.Add(Path[0]);
        for (int i = 2; i < Path.Count; i++)
        {
            int Strikes = 2;
            foreach (Vector3 neighbour1 in UtilPath.AllAdjDirections(Path[i-2]))
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
    public void MakePath(Vector3 StartLoc)
    {
        Vector3 ShortRoute = StartLoc;
        Path = new List<Vector3>();
        AddTile(ShortRoute);
        Vector3 NextRoute = RightDir(ShortRoute);
        while (ShortRoute != NextRoute)
        {
            AddTile(NextRoute);
            ShortRoute = NextRoute;
            NextRoute = RightDir(ShortRoute);
        }
        ReduceNodes();
        
    }
    private Vector3 RightDir(Vector3 Loc)
    {
        Vector3 RightOne=Loc;
        foreach (Vector3 Dir in UtilPath.AllAdjDirections(Loc))
        {
            if (ContainsKey(Dir)&&ContainsKey(RightOne))
            {
                if (GetValue(Dir) < GetValue(RightOne))
                {
                    RightOne = Dir;
                }
            }else if (ContainsKey(Dir))
            {
                RightOne = Dir;
            }
        }

        return RightOne;
    }
}
[Serializable]
public struct TravelPoints
{
    public List<Paths> CheckPoints;

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
