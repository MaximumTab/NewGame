using System;
using System.Collections.Generic;
using UnityEngine;

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
    }
    public int ActivePath = 0;
    public bool CreatePath;
    public List<Vector3> Visited;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    public TravelPoints[] Routes;
    private void OnDrawGizmos() 
    {
        if (Routes.Length != 0)
        {
            Gizmos.color = Color.white;
            if (CreatePath)
            {
                CreatePath = false;
                Path[] Tiles = transform.parent.GetComponentsInChildren<Path>();
                Debug.Log(Tiles.Length);
                for (int i = 0; i < Routes[ActivePath].CheckPoints.Count; i++)
                {
                    Visited = new List<Vector3>();
                    Routes[ActivePath].CheckPoints[i].EmpAndAddDist(Tiles);

                    AddValues(Routes[ActivePath].CheckPoints[i].Objective.position, 1, i);
                }
            }
            MakePath();
        }
    }

    private void AddValues(Vector3 StartLoc, int Index,int i)
    {
        Visited.Add(StartLoc);
        if (StartLoc != transform.position)
        {
            foreach (Vector3 Dir in AllDirections(StartLoc))
            {
                Debug.Log("Checking " + Dir + " for Index" + Index);
                if (Routes[ActivePath].CheckPoints[i].ContainsKey(Dir) && !Visited.Contains(Dir))
                {
                    Routes[ActivePath].CheckPoints[i].SetValue(Dir,Index);
                    Debug.Log("Added "+Dir+" with Index "+Index);
                    AddValues(Dir,Index+1,i);
                }
                else if(Routes[ActivePath].CheckPoints[i].ContainsKey(Dir)&&Routes[ActivePath].CheckPoints[i].GetValue(Dir)>Index)
                {
                    Routes[ActivePath].CheckPoints[i].SetValue(Dir,Index);
                    Debug.Log("Added "+Dir+" with Index "+Index);
                    AddValues(Dir,Index+1,i);
                }
            }
        }
        Debug.Log("Exiting "+Index);
    }

    private void MakePath()
    {
        Vector3 ShortRoute = transform.position;
        for(int i=0;i<Routes[ActivePath].CheckPoints.Count;i++)
        {
            Vector3 NextRoute = RightDir(ShortRoute,i);
            while (ShortRoute != NextRoute)
            {
                Gizmos.DrawLine(ShortRoute, NextRoute);
                ShortRoute = NextRoute;
                NextRoute = RightDir(ShortRoute,i);
            }

            Debug.Log("Made Path to "+i);
        }
    }

    private Vector3 RightDir(Vector3 Loc,int i)
    {
        Vector3 RightOne=Loc;
        foreach (Vector3 Dir in AllDirections(Loc))
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

    private Vector3[] AllDirections(Vector3 Loc)
    {
        return new Vector3[]
        {
            Loc + Vector3.forward,
            Loc + Vector3.back,
            Loc + Vector3.left,
            Loc + Vector3.right
        };
    }
}
