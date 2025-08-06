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
    

    public List<TravelPoints> Routes;

    public void GeneratePath()
    {
        for (int k = 0; k < Routes.Count; k++)
        {
            //Routes[k].CheckPoints.First().Distances=new Dictionary<Vector3, int>();
            Path[] Tiles = transform.parent.GetComponentsInChildren<Path>();
            for (int i = 0; i < Routes[k].CheckPoints.Count; i++)
            {
                Visited = new List<Vector3>();
                Routes[k].CheckPoints[i].Distances =UtilPath.EmpAndAddDist(Routes[k].CheckPoints[i].Distances,Tiles);

                AddValues(Routes[k].CheckPoints[i].Objective.position, 1, i,k);
                Routes[k].CheckPoints[i].Path=UtilPath.MakePath(i==0?transform.position:Routes[k].CheckPoints[i-1].Objective.transform.position,Routes[k].CheckPoints[i].Distances,Routes[k].CheckPoints[i].Path);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (Routes.Count != 0)
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
                if (Routes[RouteID].CheckPoints[i].Distances.ContainsKey(Dir) && !Visited.Contains(Dir))
                {
                    Routes[RouteID].CheckPoints[i].Distances[Dir]=Index;
                    AddValues(Dir,Index+1,i,RouteID);
                }
                else if(Routes[RouteID].CheckPoints[i].Distances.ContainsKey(Dir)&&Routes[RouteID].CheckPoints[i].Distances[Dir]>Index)
                {
                    Routes[RouteID].CheckPoints[i].Distances[Dir]=Index;
                    AddValues(Dir,Index+1,i,RouteID);
                }
            }
        }
    }
}

