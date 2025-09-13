using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBehaviour : EntityBehaviour
{
    public TravelPoints Route;
    private List<Vector3> CurPath;
    private List<Vector3> Visited;
    public Dictionary<Vector3,Tunnel> TunnelLocs;
    public List<float> TunnelTimes;
    public int TunnelIndex = 0;
    private GameManager GM;
    private Collider col;
    protected int Lives;
    private EnemyStats myStats;
    private bool Down = false;

    protected int CheckProg;
    protected bool Leaked;
    public bool SpawnSuccess = false;
    private bool Waits = false;
    private float FullLength;

    public bool ForLeak;

    public override void OnSpawn()
    {
        myStats = (EnemyStats)entityStats;
        gameObject.layer = 6;
        CreateCollider();
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        CheckProg = 0;
        Leaked = false;
        Lives = 0;
        base.OnSpawn();
        NewRoute();
        if (!GM)
        {
            GM = FindFirstObjectByType<GameManager>();
        }
    }
    
    public virtual void CreateCollider()
    {
        if (col)
        {
            Destroy(col);
        }

        switch (myStats.StageAmounts[Lives])
        {
            case EnemyStats.MoveType.Moving:
                CapsuleCollider CapCol=gameObject.AddComponent<CapsuleCollider>();
                CapCol.radius = 0.05f;
                CapCol.height = 0.2f;
                CapCol.center.Set(0,0.2f,0);
                col = CapCol;
                break;
            case EnemyStats.MoveType.Stationary:
                BoxCollider BoxCol = gameObject.AddComponent<BoxCollider>();
                BoxCol.size = myStats.StationaryHBoxSize[GetStationaryLife()];
                BoxCol.center = myStats.StationaryHBoxCenter[GetStationaryLife()];
                col = BoxCol;
                StartCoroutine(nameof(StationTime));
                break;
        }
    }

    public int GetStationaryLife()
    {
        return myStats.StageAmounts.GetRange(0, Lives + 1).FindAll(type => type == EnemyStats.MoveType.Stationary)
            .Count - 1;
    }

    public override void AlwaysRun()
    {
        base.AlwaysRun();
        FollowingPath();
    }

    public void Move()
    {
        rb.linearVelocity=(CurPath.First()-transform.position).normalized * (Attacking||Blocked||Down||myStats.StageAmounts[Lives]==EnemyStats.MoveType.Stationary?0:Speed);
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

    public virtual void ReachedCheckPoint()
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
            GM.LoseALife(myStats.ObjectiveLives);
            DestroySelf();
        }
    }

    private IEnumerator Waiting()
    {
        float time=0;
        Waits = true;
        yield return new WaitForSeconds(Route.CheckPoints[CheckProg - 1].WaitSeconds);
        NewRoute();
        Waits = false;
        yield return null;
    }

    public void FollowingPath()
    {
        if (CurPath.Count != 0&&SpawnSuccess&&!Waits)
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

    public override void DoAction()
    {
        if (!Down)
        {
            base.DoAction();
        }
    }
 
    private IEnumerator StationTime()
    {
        yield return new WaitForSeconds(myStats.StationaryAliveTime[GetStationaryLife()]);
        if (Lives+1 != myStats.Lives)
        {
            StartCoroutine(NewLife());
        }
        else
        {
            Leaked = true;
            rb.linearVelocity = Vector3.zero;
            GM.LoseALife(myStats.ObjectiveLives);
            DestroySelf();
        }
    }

    public void GoingThroughTunnel()
    {
        if (TunnelLocs.ContainsKey(CurPath[0]) && TunnelLocs.ContainsKey(CurPath[1]))
        {
            foreach (Tunnel Buds in TunnelLocs[CurPath[0]].ExitTunnel)
            {
                if (Buds.transform.position == CurPath[1])
                {
                    TunnelManager.GoingThrough(gameObject, CurPath[1], TunnelTimes[TunnelIndex++]);
                }
            }
        }
    }
    public override void CheckAlive()
    {
        if (Hp <= 0&&Lives+1==myStats.Lives&&!Down)
        {
            foreach (GameObject other in BlockingTargets.Keys)
            {
                if (other.GetComponent<EntityBehaviour>())
                {
                    EntityBehaviour otherEnt = other.GetComponent<EntityBehaviour>();
                    if (otherEnt.Stats.Tag != entityStats.Tag)
                    {
                        if (otherEnt.Stats.Tag == EntityStats.ObjectTag.Enemy)
                        {
                            otherEnt.Blocked = false;
                        }
                        otherEnt.BlockingTargets.Remove(gameObject);
                    }
                }
            }
            DestroySelf();
        }else if (Lives+1 != myStats.Lives&&Hp<=0)
        {
            Hp = 1;
            StopCoroutine(nameof(StationTime));
            StartCoroutine(NewLife());
        }
        PercHp = Hp / MaxHp;
        if(HpSlider)
            HpSlider.value = PercHp;
    }

    public override void SetStats()
    {
        MaxHp = entityStats.MaxHp*myStats.MaxHpMod[Lives];
        Atk = entityStats.Atk*myStats.AtkMod[Lives];
        Speed = entityStats.Speed*myStats.SpdMod[Lives];
        AtkInterval = entityStats.AttackInterval;
        Aspd = 100*myStats.AtkspdMod[Lives];
    }

    IEnumerator NewLife()
    {
        Down = true;
        if (col)
        {
            Destroy(col);
        }
        Lives++;
        SetStats();
        for (float DownVar = 0; DownVar <= 1; DownVar += Time.deltaTime/ myStats.DownTime[Lives - 1])
        {
            Hp = Mathf.Lerp(Hp, MaxHp, DownVar);
            yield return null;
        }
        CreateCollider();
        Down = false;
    }

    public override void TakeDamage(float DamageTaken)
    {
        if(!Down)
            base.TakeDamage(DamageTaken);
    }

    public override void DestroySelf()
    {
        if (ForLeak)
        {
            GM.SetCurEnemCount();
        }

        base.DestroySelf();
    }
}
