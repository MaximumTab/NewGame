using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityBehaviour : MonoBehaviour
{
    [SerializeField] protected EntityStats entityStats;
    public float Hp;
    protected float MaxHp;
    protected float Atk;
    protected float AtkInterval;
    protected float Aspd;
    protected float Speed;
    public float Order;
    protected bool Attacking = false;
    protected bool Blocked = false;
    protected bool[] AbilityOnCooldown;
    protected List<GameObject>[] TargetsInRange;
    private class CoEntManager: MonoBehaviour { }


    private CoEntManager CoEntMan;

    public virtual void OnSpawn()
    {
        if (!CoEntMan)
        {
            GameObject CoMan = new GameObject("CoEntityManager for "+entityStats.Name);
            CoEntMan=CoMan.AddComponent<CoEntManager>();
        }
        gameObject.tag = entityStats.Tag.ToString();
        Hp = entityStats.MaxHp;
        MaxHp = entityStats.MaxHp;
        Atk = entityStats.Atk;
        Speed = entityStats.Speed;
        AtkInterval = entityStats.AttackInterval;
        Aspd = 100;
        AbilityOnCooldown = new bool[entityStats.Abilities.Length];
        TargetsInRange = new List<GameObject>[entityStats.Abilities.Length];
    }

    private void sortTarget()
    {
        for (int i = 0; i < TargetsInRange.Length; i++)
        {
            switch (entityStats.SortBy.Stat)
            {
                case EntityStats.SortedBy.Stats.Hp:
                    TargetsInRange[i].Sort(((o, o1) =>o.GetComponent<EntityBehaviour>().Hp.CompareTo(o1.GetComponent<EntityBehaviour>().Hp) ));
                    break;
                case EntityStats.SortedBy.Stats.Atk:
                    TargetsInRange[i].Sort(((o, o1) =>o.GetComponent<EntityBehaviour>().Atk.CompareTo(o1.GetComponent<EntityBehaviour>().Atk) ));
                    break;
                case EntityStats.SortedBy.Stats.Order:
                    TargetsInRange[i].Sort(((o, o1) =>o.GetComponent<EntityBehaviour>().Order.CompareTo(o1.GetComponent<EntityBehaviour>().Order) ));
                    break;
            }
        }
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
            TargetsInRange[index] = new List<GameObject>();
            foreach (Collider other in Physics.OverlapSphere(transform.position, entityStats.Abilities[index].Ability.Range))
            {
                if (other.gameObject.CompareTag(entityStats.Tag == EntityStats.ObjectTag.Enemy ? EntityStats.ObjectTag.Tower.ToString() : EntityStats.ObjectTag.Enemy.ToString())&&!other.isTrigger)
                {
                    TargetsInRange[index].Add(other.gameObject);
                }
            }
            sortTarget();
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
            DestroySelf();
        }
    }

    public void OverMaxHp()
    {
        if (Hp > MaxHp)
        {
            Hp=MaxHp;
        }
    }

    public void DestroySelf()
    {
        Destroy(CoEntMan.gameObject);
        Destroy(gameObject);
    }

}
