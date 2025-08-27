using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EntityBehaviour : MonoBehaviour
{
    [SerializeField] protected GameObject HpBar;
    private Slider HpSlider;
    private Camera SceneCam;
    [SerializeField] protected EntityStats entityStats;
    public float Hp;
    public float PercHp;
    protected float MaxHp;
    protected float Atk;
    protected float AtkInterval;
    protected float Aspd;
    protected float Speed;
    public float Order;
    protected bool Attacking = false;
    protected bool[] AbilityOnCooldown;
    protected List<GameObject>[] TargetsInRange;
    protected Dictionary<GameObject,int> BlockingTargets;
    public bool Blocked = false;
    public Rigidbody rb;
    private class CoEntManager: MonoBehaviour { }


    private CoEntManager CoEntMan;

    public virtual void OnSpawn()
    {
        if (!CoEntMan)
        {
            GameObject CoMan = new GameObject("CoEntityManager for "+entityStats.Name);
            CoEntMan=CoMan.AddComponent<CoEntManager>();
        }
        CreateHpBar();
        gameObject.tag = entityStats.Tag.ToString();
        Hp = entityStats.MaxHp;
        PercHp = 1;
        MaxHp = entityStats.MaxHp;
        Atk = entityStats.Atk;
        Speed = entityStats.Speed;
        AtkInterval = entityStats.AttackInterval;
        Aspd = 100;
        AbilityOnCooldown = new bool[entityStats.Abilities.Length];
        TargetsInRange = new List<GameObject>[entityStats.Abilities.Length];
        BlockingTargets = new Dictionary<GameObject, int>();
    }

    public virtual void CreateHpBar()
    {
        if (!HpSlider)
        {
            GameObject hpBar= Instantiate(HpBar, transform);
            HpSlider = hpBar.GetComponentInChildren<Slider>();
            SceneCam = FindFirstObjectByType<Camera>().GetComponentInChildren<Camera>();
            hpBar.GetComponentInChildren<Canvas>().worldCamera = SceneCam;
            hpBar.transform.rotation = SceneCam.transform.rotation;
        }
    }


    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<EntityBehaviour>())
        {
            EntityBehaviour otherEnt = other.gameObject.GetComponent<EntityBehaviour>();
            if (otherEnt.entityStats.Tag != entityStats.Tag)
            {
                if (otherEnt.entityStats.Tag == EntityStats.ObjectTag.Enemy)
                {
                    otherEnt.Blocked = false;
                }
                otherEnt.BlockingTargets.Remove(gameObject);
                BlockingTargets.Remove(otherEnt.gameObject);
                if (BlockingTargets.Count == 0)
                {
                    Blocked = false;
                }
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EntityBehaviour>())
        {
            EntityBehaviour otherEnt = other.gameObject.GetComponent<EntityBehaviour>();
            if (otherEnt.entityStats.Tag != entityStats.Tag)
            {
                if (otherEnt.entityStats.Tag == EntityStats.ObjectTag.Enemy&&!otherEnt.Blocked&&BlockingTargets.Values.Sum()+otherEnt.entityStats.Block<=entityStats.Block)
                {
                    otherEnt.Blocked = true;
                    otherEnt.BlockingTargets.Add(gameObject,0);
                    BlockingTargets.Add(otherEnt.gameObject,otherEnt.entityStats.Block);
                    Blocked = true;
                }
            }
        }
    }

    private List<GameObject> sortTarget(List<GameObject> list)
    {
        List<GameObject> ResultList = new List<GameObject>();
        switch (entityStats.SortBy.Stat)
        {
            case EntityStats.SortedBy.Stats.Hp:
                switch (entityStats.SortBy.Method)
                {
                    case EntityStats.SortedBy.Methods.Greatest:
                        ResultList=new List<GameObject>(list.OrderByDescending(o => o.GetComponent<EntityBehaviour>().Hp));
                        break;
                    case EntityStats.SortedBy.Methods.None:
                    case EntityStats.SortedBy.Methods.Smallest:
                        ResultList=new List<GameObject>(list.OrderBy(o => o.GetComponent<EntityBehaviour>().Hp));
                        break;
                }
                break;
            case EntityStats.SortedBy.Stats.Atk:
                switch (entityStats.SortBy.Method)
                {
                    case EntityStats.SortedBy.Methods.Greatest:
                        ResultList=new List<GameObject>(list.OrderByDescending(o => o.GetComponent<EntityBehaviour>().Atk));
                        break;
                    case EntityStats.SortedBy.Methods.None:
                    case EntityStats.SortedBy.Methods.Smallest:
                        ResultList=new List<GameObject>(list.OrderBy(o => o.GetComponent<EntityBehaviour>().Atk));
                        break;
                }
                break;
            case EntityStats.SortedBy.Stats.Order:
                switch (entityStats.SortBy.Method)
                {
                    case EntityStats.SortedBy.Methods.Greatest:
                        ResultList=new List<GameObject>(list.OrderByDescending(o => o.GetComponent<EntityBehaviour>().Order));
                        break;
                    case EntityStats.SortedBy.Methods.None:
                    case EntityStats.SortedBy.Methods.Smallest:
                        ResultList=new List<GameObject>(list.OrderBy(o => o.GetComponent<EntityBehaviour>().Order));
                        break;
                }
                break;
        }

        return ResultList;
    }

    private void Update()
    {
        AlwaysRun();
    }

    public virtual void AlwaysRun()
    {
        DoAction();
        CheckAlive();
        OverMaxHp();
    }

    public virtual void DoAction()
    {
        for (int index=0;index<entityStats.Abilities.Length;index++)
        {
            if (entityStats.Abilities[index].Range == EntityStats.RangeType.Melee && !Blocked)
            {
                continue;
            }

            TargetsInRange[index] = new List<GameObject>();
            TargetsInRange[index].AddRange(BlockingTargets.Keys);
            List<GameObject> TempList=new List<GameObject>();
            foreach (Collider other in Physics.OverlapSphere(transform.position, entityStats.Abilities[index].Ability.Range))
            {
                if (AblTargetConf(other.gameObject,entityStats.Abilities[index].Ability)&&!TargetsInRange[index].Contains(other.gameObject))
                {
                    TempList.Add(other.gameObject);
                }
            }
            TargetsInRange[index].AddRange(sortTarget(TempList));
            
            if (!AbilityOnCooldown[index] && !Attacking && TargetsInRange[index].Count > 0&&entityStats.Abilities[index].Ability.GetType()!=typeof(SummonerAbil))
            {
                CoEntMan.StartCoroutine(WaitAttacks());
                CoEntMan.StartCoroutine(CoolDownAbl(index));
                //Add way to tie into animation.
                for (int i = 0;
                     i < (entityStats.Abilities[index].Ability.NumOfTargets < TargetsInRange[index].Count
                         ? entityStats.Abilities[index].Ability.NumOfTargets
                         : TargetsInRange[index].Count);
                     i++)
                {
                    entityStats.Abilities[index].Ability.UseAbility(TargetsInRange[index][i], transform.position, Atk);
                }
            }
            else if (!AbilityOnCooldown[index] && !Attacking&&entityStats.Abilities[index].Ability.GetType()==typeof(SummonerAbil))
            {
                CoEntMan.StartCoroutine(WaitAttacks());
                CoEntMan.StartCoroutine(CoolDownAbl(index));
                //Add way to tie into animation.
                for (int i = 0; i < entityStats.Abilities[index].Ability.NumOfTargets;i++)
                {
                    entityStats.Abilities[index].Ability.UseAbility(gameObject, transform.position, Atk);
                }
            }
        }
    }

    private bool AblTargetConf(GameObject other, Ability abl)
    {
        return other.CompareTag(AblTargetString(abl));
    }

    private string AblTargetString(Ability abl)
    {
        if (abl.GetType() != typeof(HealerAbil))
        {
            if (entityStats.Tag == EntityStats.ObjectTag.Enemy)
            {
                return EntityStats.ObjectTag.Tower.ToString();
            }
            return EntityStats.ObjectTag.Enemy.ToString();
        }
        if (entityStats.Tag == EntityStats.ObjectTag.Enemy)
        {
            return EntityStats.ObjectTag.Enemy.ToString();
        }
        return EntityStats.ObjectTag.Tower.ToString();
    }

    public IEnumerator WaitAttacks()
    {
        Attacking = true;
        yield return new WaitForSeconds(AtkInterval);
        Attacking = false;
    }

    public IEnumerator CoolDownAbl(int abilityIndex)
    {
        AbilityOnCooldown[abilityIndex] = true;
        yield return new WaitForSeconds(entityStats.Abilities[abilityIndex].Ability.Cooldown);
        AbilityOnCooldown[abilityIndex] = false;
    }

    public void TakeDamage(float DamageTaken)
    {
        Hp -= DamageTaken;
    }

    public void CheckAlive()
    {
        if (Hp <= 0)
        {
            foreach (GameObject other in BlockingTargets.Keys)
            {
                if (other.GetComponent<EntityBehaviour>())
                {
                    EntityBehaviour otherEnt = other.GetComponent<EntityBehaviour>();
                    if (otherEnt.entityStats.Tag != entityStats.Tag)
                    {
                        if (otherEnt.entityStats.Tag == EntityStats.ObjectTag.Enemy)
                        {
                            otherEnt.Blocked = false;
                        }
                        otherEnt.BlockingTargets.Remove(gameObject);
                    }
                }
            }
            DestroySelf();
            
        }
        PercHp = Hp / MaxHp;
        if(HpSlider)
            HpSlider.value = PercHp;
    }

    public void OverMaxHp()
    {
        if (Hp > MaxHp)
        {
            Hp=MaxHp;
        }
    }

    public virtual void DestroySelf()
    {
        Destroy(CoEntMan.gameObject);
        Destroy(gameObject);
    }

}
