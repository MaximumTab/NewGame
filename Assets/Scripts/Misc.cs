using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Paths
{
    public Transform Objective;
    public Dictionary<Vector3, int> Distances;
    public List<Vector3> Path;
    public float WaitSeconds;

    public Paths(Transform objective)
    {
        Objective = objective;
    }
    public void OnDrawGizmos()
    {
        for (int i = 0; i < Path.Count - 1; i++)
        {
            Gizmos.DrawLine(Path[i],Path[i+1]);
        }
    }
}
[Serializable]
public class TravelPoints
{
    public List<Paths> CheckPoints;

    public TravelPoints()
    {
        
    }

    public float GetFullPathLength()
    {
        float result = 0;
        foreach (Paths route in CheckPoints)
        {
            result += UtilPath.GetPathLength(route.Path);
        }

        return result;
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

    public static Dictionary<Vector3, int> EmpAndAddDist(Dictionary<Vector3, int> Distances,Path[] Tiles)
    {
        Distances = new Dictionary<Vector3, int>();
        foreach (Path path in Tiles)
        {
            Distances.Add(path.transform.position,0);
        }

        return Distances;
    }

    public static void AddTile(List<Vector3> Path,Vector3 Key)
    {
        if (!Path.Contains(Key))
        {
            Path.Add(Key);
        }
    }

    public static List<Vector3> ReduceNodes(List<Vector3> Path,Dictionary<Vector3, int> Distances, Dictionary<Vector3,Tunnel>Tunnels)
    {
        Path=Shortcut(Path,Distances,Tunnels);
        List<Vector3> ResultPath=new List<Vector3>();
        Vector3 LastNode=Path[0];
        Vector3 PreviousTile=Path[0];
        ResultPath.Add(LastNode);
        foreach (Vector3 Tile in Path)
        {
            if (!Mathf.Approximately(LastNode.x, Tile.x) && !Mathf.Approximately(LastNode.z, Tile.z)||Tunnels.ContainsKey(Tile))
            {
                ResultPath.Add(PreviousTile);
                LastNode = PreviousTile;
            }

            PreviousTile = Tile;
        }
        ResultPath.Add(Path.Last());
        return ResultPath;
    }

    public static List<Vector3> Shortcut(List<Vector3> Path,Dictionary<Vector3, int> Distances,Dictionary<Vector3,Tunnel>Tunnels)
    {
        List<Vector3> ResultPath=new List<Vector3>();
        ResultPath.Add(Path[0]);
        for (int i = 2; i < Path.Count; i++)
        {
            int Strikes = 2;
            foreach (Vector3 neighbour1 in AllAdjDirections(Path[i-2]))
            {
                foreach (Vector3 neighbour2 in AllAdjDirections(Path[i]))
                {
                    if (neighbour1 == neighbour2 && Distances.ContainsKey(neighbour1))
                    {
                        Strikes--;
                    }
                }
            }
            if (Strikes != 0||Tunnels.ContainsKey(Path[i-1]))
            {
                ResultPath.Add(Path[i-1]);
            }
        }
        ResultPath.Add(Path.Last());
        return ResultPath;
        
    }
    public static  List<Vector3> MakePath(Vector3 StartLoc,Dictionary<Vector3, int> Distances,List<Vector3>Path, Dictionary<Vector3,Tunnel> Tunnels)
    {
        Vector3 ShortRoute = StartLoc;
        Path = new List<Vector3>();
        AddTile(Path,ShortRoute);
        Vector3 NextRoute = RightDir(ShortRoute,Distances,Tunnels);
        while (ShortRoute != NextRoute||Tunnels.ContainsKey(ShortRoute))
        {
            AddTile(Path,NextRoute);
            ShortRoute = NextRoute;
            NextRoute = RightDir(ShortRoute,Distances,Tunnels);
        }
        return ReduceNodes(Path,Distances,Tunnels);
        
    }
    private static Vector3 RightDir(Vector3 Loc,Dictionary<Vector3, int> Distances,Dictionary<Vector3,Tunnel>Tunnels)
    {
        Vector3 RightOne=Loc;
        Debug.Log("Tunnel Count is "+Tunnels.Count);
        if (Tunnels.Keys.Contains(Loc))
        {
            Debug.Log("Gone through Tunnels");
            if (Distances.Keys.Contains(Tunnels[Loc].BuddyTunnel.transform.position) && Distances.Keys.Contains(RightOne))
            {
                if (Distances[Tunnels[Loc].BuddyTunnel.transform.position] < Distances[RightOne])
                {
                    return Tunnels[Loc].BuddyTunnel.transform.position;
                }
            }
        }
        foreach (Vector3 Dir in AllAdjDirections(Loc))
        {
            if (Distances.Keys.Contains(Dir)&&Distances.Keys.Contains(RightOne))
            {
                if (Distances[Dir] < Distances[RightOne])
                {
                    RightOne = Dir;
                }
            }else if (Distances.Keys.Contains(Dir))
            {
                RightOne = Dir;
            }
        }

        return RightOne;
    }

    public static float GetPathLength(List<Vector3> Path)
    {
        float result = 0;
        for (int i = 1; i < Path.Count; i++)
        {
            result += (Path[i - 0] - Path[i]).magnitude;
        }

        return result;
    }
}
