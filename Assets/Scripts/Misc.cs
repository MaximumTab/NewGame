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
    private static Vector3[] Dirs = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right,Vector3.forward + Vector3.left,Vector3.back + Vector3.right,Vector3.left + Vector3.back,Vector3.right + Vector3.forward };
    private static Vector3[] AdjDirs = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
    public static Vector3[] AllAdjDirections(Vector3 Loc)
    {
        Vector3[] Directions = new Vector3[4];
        for (int i = 0; i < AdjDirs.Length; i++)
        {
            Directions[i] = Loc+AdjDirs[i];
        }

        return Directions;
    }
    public static Vector3[] AllDirections(Vector3 Loc,int Closeness)
    {
        if (Closeness == 1)
        {
            Vector3[] Directions = new Vector3[8];
            for (int i = 0; i < Dirs.Length; i++)
            {
                Directions[i] = Loc + Dirs[i];
            }

            return Directions;
        }
        else
        {
            return AllAdjDirections(Loc);
        }
    }

    public static Dictionary<Vector3, int> EmpAndAddDist(Dictionary<Vector3, int> Distances,Path[] Tiles)
    {
        Distances = new Dictionary<Vector3, int>();
        foreach (Path path in Tiles)
        {
            Distances.Add(path.transform.position,Int32.MaxValue);
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
            if (!Mathf.Approximately(LastNode.x, Tile.x) && !Mathf.Approximately(LastNode.z, Tile.z)||Tunnels.ContainsKey(PreviousTile))
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
        int LastDead = 0;
        int checkNeighbours = 0;
        for (int i = 2; i < Path.Count; i++)
        {
            float Strikes = 2;
            foreach (Vector3 LateNode in AllDirections(Path[i-2-LastDead],checkNeighbours))
            {
                foreach (Vector3 EarlyNode in AllDirections(Path[i],checkNeighbours))
                {
                    if (!Distances.ContainsKey(LateNode) || !Distances.ContainsKey(EarlyNode))
                        continue;
                    if ((Path[i - 2 - LastDead] - Path[i]).magnitude <= 2)
                    {
                        if (LateNode == EarlyNode&&!(Path[i - 2 ].x == Path[i ].x ||
                              Path[i - 2 ].z == Path[i ].z))//CornerCases
                        {
                            Strikes--;
                        }
                        else if((Path[i] - EarlyNode).magnitude == 1&&(Path[i - 2 ] - LateNode).magnitude == 1&&(EarlyNode-LateNode).magnitude==2&&(Path[i - 2 ].x == Path[i ].x ||
                                    Path[i - 2 ].z == Path[i ].z)&&EarlyNode!=Path[i-1]&&LateNode!=Path[i-1])
                        {
                            if(Distances.ContainsKey((EarlyNode-LateNode)/2+EarlyNode)&&Distances.ContainsKey(Path[i]*2-EarlyNode)==Distances.ContainsKey(Path[i - 2 ]*2 - LateNode))
                            {
                                Strikes-=2;
                            }
                        }
                    }else if ((Path[i - 2 - LastDead] - Path[i]).magnitude > 2)
                    {
                        if (LateNode == EarlyNode)
                        {
                            Strikes -= 0.5f;
                        }

                        if ((Path[i - 2 - LastDead] - EarlyNode).magnitude == 2 && (Path[i] - LateNode).magnitude == 2)
                        {
                            Strikes--;
                            if (Path.Contains(Path[i - 2 - LastDead] * 2 - LateNode)&&!Path.Contains(Path[i] * 2 - EarlyNode))
                            {
                                ResultPath.Add((Path[i - 2 - LastDead] - EarlyNode) / 2 + EarlyNode);
                            }/*
                            else if(Path.Contains(Path[i] * 2 - EarlyNode))
                            {
                                ResultPath.Add((Path[i] - LateNode) / 2 + LateNode);
                            }*/
                        }
                    }
                }
            }
            if (Strikes <= 0)
            {
                LastDead = 1;
                checkNeighbours = 1;
            }else if(LastDead==1)
            {
                LastDead = 0;
                i--;
            }
            else
            {
                checkNeighbours = 0;
                ResultPath.Add(Path[i-1]);
            }

            /*if (Strikes != 0||Tunnels.ContainsKey(Path[i-1]))
            {
                ResultPath.Add(Path[i-1]);
                LastDead = 0;
            }
            else
            {
                LastDead=1;
            }*/
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
        //Debug.Log("Tunnel Count is "+Tunnels.Count);
        if (Tunnels.Keys.Contains(Loc))
        {
            //Debug.Log("Gone through Tunnels");
            foreach(Tunnel Buds in Tunnels[Loc].ExitTunnel)
            {
                if (Distances.Keys.Contains(Buds.transform.position) && Distances.Keys.Contains(RightOne))
                {
                    if (Distances[Buds.transform.position] < Distances[RightOne])
                    {
                        return Buds.transform.position;
                    }
                }
            }
            
        }
        foreach (Vector3 Dir in Distances[Loc] % 2 == 0 ? AllAdjDirections(Loc) : AllAdjDirections(Loc).Reverse())
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
