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
    
    private int CheckProg;
    private bool Leaked;
    public bool SpawnSuccess = false;
    private bool Waits = false;

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
    }

    public void NewRoute()
    {
        //Route.CheckPoints[CheckProg].MakePath(transform.position);
        CurPath = Route.CheckPoints[CheckProg].Path;
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
            Debug.Log("Ive reached the end!!");
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
                CurPath.Remove(CurPath.First());
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
}
