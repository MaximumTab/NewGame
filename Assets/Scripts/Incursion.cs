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

