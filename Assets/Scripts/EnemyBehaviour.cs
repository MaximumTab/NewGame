using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField] private EnemyStats EnemyStats;
    private Rigidbody rb;
    public TravelPoints Route;
    public List<Vector3> CurPath;
    
    //Stats Manipulation Area
    public int CheckProg;
    private float Hp;
    private float Atk;
    private float Speed;
    private float AtkInterval;
    private float Aspd;
    private bool Leaked;
    public bool SpawnSuccess = false;
    private bool Waits = false;

    public void OnSpawn()
    {
        Hp = EnemyStats.MaxHp;
        Atk = EnemyStats.Atk;
        Speed = EnemyStats.Speed;
        AtkInterval = EnemyStats.AttackInterval;
        Aspd = 100;
        CheckProg = 0;
        Leaked = false;
        rb = gameObject.GetComponent<Rigidbody>();
        NewRoute();
    }

    public void TakeDamage(float DamageTaken)
    {
        Hp -= DamageTaken;
    }

    public bool CheckAlive()
    {
        return Hp > 0;
    }

    //SMA ends here

    private void Update()
    {
        AlwaysRun();
    }

    public void AlwaysRun()
    {
        FollowingPath();
    }

    public void Move()
    {
        rb.linearVelocity=(CurPath.First()-transform.position).normalized * Speed;

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
