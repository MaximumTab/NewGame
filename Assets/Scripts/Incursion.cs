using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
public class Incursion:MonoBehaviour
{
    public bool CreatePath;
    public List<Vector3> Visited;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public Dictionary<Vector3, Tunnel> Tunnels;
    public List<TravelPoints> Routes;

    protected Incursion()
    {
        Routes = new List<TravelPoints>();
    }

    public void GeneratePath()
    {
        //GameManager.MakingIGM();
        Path[] Tiles = transform.parent.GetComponentsInChildren<Path>();
        Tunnel[] TunnTiles = transform.parent.GetComponentsInChildren<Tunnel>();
        Debug.Log(Tiles.Length+ " "+TunnTiles.Length);
        Tunnels = new Dictionary<Vector3, Tunnel>();
        foreach (Tunnel Tun in TunnTiles)
        {
            Tunnels.Add(Tun.transform.position,Tun);
        }
        for (int k = 0; k < Routes.Count; k++)
        {
            //Routes[k].CheckPoints.First().Distances=new Dictionary<Vector3, int>();
            for (int i = 0; i < Routes[k].CheckPoints.Count; i++)
            {
                Visited = new List<Vector3>();
                Routes[k].CheckPoints[i].Distances =UtilPath.EmpAndAddDist(Routes[k].CheckPoints[i].Distances,Tiles);
                Routes[k].CheckPoints[i].Distances[Routes[k].CheckPoints[i].Objective.position]=0;
                AddValues(Routes[k].CheckPoints[i].Objective.position, 1, i,k);
                Routes[k].CheckPoints[i].Path=UtilPath.MakePath(i==0?transform.position:Routes[k].CheckPoints[i-1].Objective.transform.position,Routes[k].CheckPoints[i].Distances,Routes[k].CheckPoints[i].Path,Tunnels);
            }
        }
    }
    public virtual void OnDrawGizmos()
    {
        if (Routes.Count != 0)
        {
            if (CreatePath)
            {
                CreatePath = false;
                GeneratePath();
            }
        }

        foreach (TravelPoints TP in Routes)
        {
            foreach (Paths drawPaths in TP.CheckPoints)
            {
                drawPaths.OnDrawGizmos();
            }
        }
    }

    private void AddValues(Vector3 StartLoc, int Index,int i,int RouteID)
    {
        Visited.Add(StartLoc);
        if (StartLoc != transform.position)
        {
            if (Tunnels.Keys.Contains(StartLoc))
            {
                foreach (Tunnel Buds in Tunnels[StartLoc].EnterTunnel)
                {
                    if (Routes[RouteID].CheckPoints[i].Distances
                            .ContainsKey(Buds.transform.position) &&
                        (!Visited.Contains(Buds.transform.position) || Routes[RouteID]
                            .CheckPoints[i].Distances[Buds.transform.position] > Index))
                    {
                        Routes[RouteID].CheckPoints[i].Distances[Buds.transform.position] =
                            Index;
                        AddValues(Buds.transform.position, Index + 1, i, RouteID);

                    }
                }

                //Debug.Log("Gone through Tunnels, my index is "+Index);
            }
            //Debug.Log("My index is "+Index);
            foreach (Vector3 Dir in UtilPath.AllAdjDirections(StartLoc))
            {
                if (Routes[RouteID].CheckPoints[i].Distances.ContainsKey(Dir) && (!Visited.Contains(Dir)||Routes[RouteID].CheckPoints[i].Distances[Dir]>Index))
                {
                    Routes[RouteID].CheckPoints[i].Distances[Dir]=Index;
                    AddValues(Dir,Index+1,i,RouteID);
                }
            }
        }
    }
}

