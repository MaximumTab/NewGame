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
    private bool Attacking = false;
    private bool Blocked = false;
    private Dictionary<Collider,int> AbilityTriggers;
    private bool[] AbilityOnCooldown;
    private List<GameObject>[] TargetsInRange;

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
        AbilityTriggers = new Dictionary<Collider, int>();
        int i = 0;
        foreach (EnemyStats.Ability statsAbility in EnemyStats.Abilities)
        {
            SphereCollider SphCol = gameObject.AddComponent<SphereCollider>();
            SphCol.radius = statsAbility.AtkExecute.Range;
            SphCol.isTrigger = true;
            AbilityTriggers.Add(SphCol,i);
            i++;
        }
        TargetsInRange = new List<GameObject>[i];
        AbilityOnCooldown = new bool[i];
        for (int k = 0; k < i; k++)
        {
            TargetsInRange[k] = new List<GameObject>();
        }

        
        NewRoute();
    }

    public void DoAction()
    {
        foreach (int index in AbilityTriggers.Values)
        {
            if (!AbilityOnCooldown[index] && !Attacking&&TargetsInRange[index].Count>0)
            {
                StartCoroutine(WaitAttacks());
                StartCoroutine(CoolDownAbl(index));
                //Add way to tie into animation.
                EnemyStats.Abilities[index].AtkExecute.UseAbility(TargetsInRange[index].First(),transform.position);
            }
        }
    }

    IEnumerator WaitAttacks()
    {
        Attacking = true;
        Debug.Log("Attacking");
        yield return new WaitForSeconds(AtkInterval);
        Attacking = false;
    }

    IEnumerator CoolDownAbl(int abilityIndex)
    {
        AbilityOnCooldown[abilityIndex] = true;
        Debug.Log("Ability "+ abilityIndex+ " On Cooldown");
        yield return new WaitForSeconds(EnemyStats.Abilities[abilityIndex].AtkExecute.Cooldown);
        AbilityOnCooldown[abilityIndex] = false;
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

    public List<int> InRange(Collider other)
    {
        List<int> newColIndex=new List<int>();
        foreach (Collider AbilityCol in AbilityTriggers.Keys)
        {
            if (AbilityCol.bounds.Intersects(other.bounds) &&
                !TargetsInRange[AbilityTriggers[AbilityCol]].Contains(other.gameObject))
            {
                newColIndex.Add(AbilityTriggers[AbilityCol]);
            }
        }

        return newColIndex;
    }
    public List<int> OutRange(Collider other)
    {
        List<int> newColIndex=new List<int>();
        foreach (Collider AbilityCol in AbilityTriggers.Keys)
        {
            if (!AbilityCol.bounds.Intersects(other.bounds) &&
                TargetsInRange[AbilityTriggers[AbilityCol]].Contains(other.gameObject))
            {
                newColIndex.Add(AbilityTriggers[AbilityCol]);
            }
        }

        return newColIndex;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Tower"))
        {
            foreach (int Index in InRange(other))
            {
                TargetsInRange[Index].Add(other.gameObject);
                Debug.Log("Added Target to collider " + Index);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Tower"))
        {
            foreach (int Index in OutRange(other))
            {
                TargetsInRange[Index].Remove(other.gameObject);
                Debug.Log("Removed Target to collider " + Index);
            }
        }
    }
}
