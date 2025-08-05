using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EntityBehaviour : MonoBehaviour
{
    [SerializeField] protected EntityStats entityStats;
    protected float Hp;
    protected float Atk;
    protected float AtkInterval;
    protected float Aspd;
    protected float Speed;
    public float Order;
    protected bool Attacking = false;
    protected bool Blocked = false;
    protected Dictionary<Collider, int> AbilityTriggers;
    protected bool[] AbilityOnCooldown;
    protected List<GameObject>[] TargetsInRange;

    public virtual void OnSpawn()
    {
        gameObject.tag = entityStats.Tag.ToString();
        Hp = entityStats.MaxHp;
        Atk = entityStats.Atk;
        Speed = entityStats.Speed;
        AtkInterval = entityStats.AttackInterval;
        Aspd = 100;
        AbilityTriggers = new Dictionary<Collider, int>();
        int i = 0;
        foreach (EntityStats.Ability statsAbility in entityStats.Abilities)
        {
            SphereCollider SphCol = gameObject.AddComponent<SphereCollider>();
            SphCol.radius = statsAbility.AtkExecute.Range;
            SphCol.isTrigger = true;
            AbilityTriggers.Add(SphCol, i);
            i++;
        }
        TargetsInRange = new List<GameObject>[i];
        AbilityOnCooldown = new bool[i];
        for (int k = 0; k < i; k++)
        {
            TargetsInRange[k] = new List<GameObject>();
        }
    }

    private void Update()
    {
        AlwaysRun();
    }

    public virtual void AlwaysRun()
    {
        DoAction();
    }

    public void DoAction()
    {
        foreach (int index in AbilityTriggers.Values)
        {
            if (!AbilityOnCooldown[index] && !Attacking && TargetsInRange[index].Count > 0)
            {
                StartCoroutine(WaitAttacks());
                StartCoroutine(CoolDownAbl(index));
                //Add way to tie into animation.
                entityStats.Abilities[index].AtkExecute.UseAbility(TargetsInRange[index].First(), transform.position, Atk);
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
        Debug.Log("Ability " + abilityIndex + " On Cooldown");
        yield return new WaitForSeconds(entityStats.Abilities[abilityIndex].AtkExecute.Cooldown);
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
    public List<int> InRange(Collider other)
    {
        List<int> newColIndex = new List<int>();
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
        List<int> newColIndex = new List<int>();
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
        if (other.gameObject.CompareTag(entityStats.Tag == EntityStats.ObjectTag.Enemy ? EntityStats.ObjectTag.Tower.ToString() : EntityStats.ObjectTag.Enemy.ToString()))
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
        if (other.gameObject.CompareTag(entityStats.Tag == EntityStats.ObjectTag.Enemy ? EntityStats.ObjectTag.Tower.ToString() : EntityStats.ObjectTag.Enemy.ToString()))
        {
            foreach (int Index in OutRange(other))
            {
                TargetsInRange[Index].Remove(other.gameObject);
                Debug.Log("Removed Target to collider " + Index);
            }
        }
    }
}
