using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : EntityBehaviour
{
    private Rigidbody rb;
    public TravelPoints Route;
    private List<Vector3> CurPath;
    private List<Vector3> Visited;
    public Dictionary<Vector3,Tunnel> TunnelLocs;
    
    private int CheckProg;
    private bool Leaked;
    public bool SpawnSuccess = false;
    private bool Waits = false;
    private float FullLength;


    public override void OnSpawn()
    {
        CheckProg = 0;
        Leaked = false;
        rb = gameObject.GetComponent<Rigidbody>();
        base.OnSpawn();
        NewRoute();
    }

    public override void AlwaysRun()
    {
        DoAction();
        FollowingPath();
    }

    public void Move()
    {
        rb.linearVelocity=(CurPath.First()-transform.position).normalized * (Attacking?0:Speed);
        Order = FullLength + (CurPath.First() - transform.position).magnitude;
    }

    public float getCurPathLength()
    {
        float result = 0;
        for (int i = CheckProg+1; i < Route.CheckPoints.Count; i++)
        {
            Vector3 prevVect = Route.CheckPoints[i].Path.First();
            foreach (Vector3 Vects in Route.CheckPoints[i].Path)
            {
                result += (Vects - prevVect).magnitude;
                prevVect = Vects;
            }
        }
        Vector3 PrevVect = CurPath.Count!=0?CurPath.First():Vector3.zero;
        foreach (Vector3 Vects in CurPath)
        {
            result += (Vects - PrevVect).magnitude;
            PrevVect = Vects;
        }

        return result;
    }

    public void NewRoute()
    {
        //Route.CheckPoints[CheckProg].MakePath(transform.position);
        CurPath =Route.CheckPoints[CheckProg].Path;
    }

    public void ReachedCheckPoint()
    {
        CheckProg++;
        if (Route.CheckPoints.Count != CheckProg)
        {
            StartCoroutine(Waiting());
        }
        else if(!Leaked)
        {
            Leaked = true;
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator Waiting()
    {
        float time=0;
        Waits = true;
        while (Route.CheckPoints[CheckProg - 1].WaitSeconds>time)
        {
            time+= Time.fixedDeltaTime;
            yield return null;
        }
        NewRoute();
        Waits = false;
        yield return null;
    }

    public void FollowingPath()
    {
        if (CurPath.Count != 0&&SpawnSuccess)
        {
            if ((transform.position - CurPath.First()).magnitude < 0.25f)
            {
                if (CurPath.Count >= 2)
                {
                    GoingThroughTunnel();
                }

                CurPath.Remove(CurPath.First());
                FullLength = getCurPathLength();
            }
            else
            {
                Move();
            }
        }
        else if(SpawnSuccess&&!Waits)
        {
            if(!Leaked)
                ReachedCheckPoint();
        }
        else if(SpawnSuccess)
        {
            rb.linearVelocity=Vector3.zero;
        }
    }

    public void GoingThroughTunnel()
    {
        if (TunnelLocs.ContainsKey(CurPath[0]) && TunnelLocs.ContainsKey(CurPath[1]))
        {
            foreach (Tunnel Buds in TunnelLocs[CurPath[1]].ExitTunnel)
            {
                if (Buds.transform.position == CurPath[1])
                {
                    TunnelManager.GoingThrough(gameObject, CurPath[1], 1);
                }
            }
        }
    }
}
