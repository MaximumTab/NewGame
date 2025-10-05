using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ProjectileBehaviour : MonoBehaviour
{
    public ProjectileStats projectileStats;
    private GameObject target;
    private float Damage;
    private bool targetHit = false;
    private bool GettingTarget = false;
    private float BounceAmount;
    private Vector3 StartLoc;
    private List<GameObject> AlreadyHitTargets;
    private List<GameObject> TargetsInRange;
    private bool Travel;
    private Rigidbody rb;

    public bool NoLatch;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        
        StartLoc = transform.position;
        TargetsInRange = new List<GameObject>();
        AlreadyHitTargets = new List<GameObject>();
        BounceAmount = projectileStats.BounceNum;
        rb.useGravity = false;
        Travel = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            CheckOnHit();
        }
        else
        {
            StartCoroutine(DestroySelf());
        }
    }

    private void sortTarget()
    {
        TargetsInRange.Sort((o, o1) =>o.GetComponent<EntityBehaviour>().Order.CompareTo(o1.GetComponent<EntityBehaviour>().Order) );
    }
    private void MoveToTarget()
    {
        switch (projectileStats.DistanceMode)
        {
            case ProjectileStats.ProjectileType.Moving:
                StartCoroutine(Moving((target.transform.position - transform.position).magnitude / projectileStats.Speed));
                break;
            case ProjectileStats.ProjectileType.Instant:
                rb.position = target.transform.position;
                break;
            case ProjectileStats.ProjectileType.Arcing:
                StartCoroutine(Arcing((target.transform.position - transform.position).magnitude / projectileStats.Speed));

                break;
        }
    }
    IEnumerator Arcing(float time)
    {
        Vector3 StartPos = rb.position;
        for (float i = 0; i < 1; i+=Time.deltaTime/time)
        {
            rb.position = Vector3.Lerp(StartPos+Vector3.up * (projectileStats.ArcHeight * i), target.transform.position+Vector3.up * (projectileStats.ArcHeight * (1-i)), i);
            yield return null;
            if (!target)
            {
                break;
            }
        }
        if (target)
        {
            rb.position = target.transform.position;
        }
    }

    IEnumerator Moving(float time)
    {
        Vector3 StartPos = rb.position;
        for (float i = 0; i < time; i+=Time.deltaTime)
        {
            rb.position = Vector3.Lerp(StartPos, target.transform.position, i/time);
            yield return null;
            if (!target)
            {
                break;
            }
        }

        if (target)
        {
            rb.position = target.transform.position;
        }
    }

    private void CheckOnHit()
    {
        if ((target.transform.position - transform.position).magnitude <= 0.1f&&!targetHit)
        {
            targetHit = true;
            if (!GettingTarget)
            {
                OnHit();
                BounceAmount--;
            }

            if (BounceAmount > 0&&projectileStats.ImpactMode==ProjectileStats.ImpactType.Bouncing)
            {
                targetHit = false;
            }
            else
            {
                StartCoroutine(DestroySelf());
            }
        }
        else if(!targetHit&&!Travel)
        {
            MoveToTarget();
            Travel = true;
        }
    }
    private void OnHit()
    {
        switch (projectileStats.ImpactMode)
        {
            case ProjectileStats.ImpactType.Single:
                StartCoroutine(DoDamage(target));
                break;
            case ProjectileStats.ImpactType.Aoe:
                foreach (Collider other in Physics.OverlapSphere(transform.position, projectileStats.BounceRange))
                {
                    if (other.gameObject.CompareTag(target.tag))
                    {
                        StartCoroutine(DoDamage(other.gameObject));
                    }
                }

                GameObject AHEffect=Instantiate(projectileStats.AoeHitEffect, transform.position, Quaternion.identity);
                AHEffect.transform.localScale=Vector3.one*projectileStats.AoeRange;
                break;
            case ProjectileStats.ImpactType.Bouncing:
                StartCoroutine(DoDamage(target));
                StartCoroutine(GetNextTarget());
                break;
        }
    }

    private IEnumerator GetNextTarget()
    {
        GettingTarget = true;
        foreach (Collider other in Physics.OverlapSphere(transform.position, projectileStats.BounceRange))
        {
            if (other.gameObject.CompareTag(target.tag))
            {
                if (!AlreadyHitTargets.Contains(other.gameObject))
                {
                    TargetsInRange.Add(other.gameObject);
                }
            }
        }

        yield return null;
        sortTarget();
        if (TargetsInRange.Count > 0)
        {
            if(target)
                StartLoc = target.transform.position;
            target = TargetsInRange.First();
            TargetsInRange = new List<GameObject>();
            Travel = false;
        }
        else
        {
            BounceAmount = 0;
            StartCoroutine(DestroySelf());
        }

        yield return null;
        GettingTarget = false;
    }

    private IEnumerator DoDamage(GameObject target)
    {
        AlreadyHitTargets.Add(target);
        for (int i = 0; i < projectileStats.NumOfHits; i++)
        {
            if (target.transform.GetComponent<EntityBehaviour>())
            {
                target.transform.GetComponent<EntityBehaviour>().TakeDamage(Damage);
                GameObject targOHE;
                if (!NoLatch)
                {
                    targOHE = Instantiate(projectileStats.OnHitEffect, target.transform);
                }
                else
                {
                    targOHE = Instantiate(projectileStats.OnHitEffect, target.transform.position,Quaternion.identity);
                }

                targOHE.transform.Rotate(Vector3.forward, Random.Range(0, 360));
            }

            yield return new WaitForSeconds(projectileStats.DelayBtwHit);
        }

        yield return null;

    }

    public void DamageDoneTo(float Atk, float AtkMod,GameObject Target)
    {
        Damage = Atk * AtkMod;
        target = Target;
        AlreadyHitTargets = new List<GameObject>();
    }

    IEnumerator DestroySelf()
    {
        rb.linearVelocity=Vector3.zero;
        yield return new WaitForSeconds(projectileStats.DelayBtwHit*(projectileStats.NumOfHits-1));
        Destroy(gameObject);
    }
}
