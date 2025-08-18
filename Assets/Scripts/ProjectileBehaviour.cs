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

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        StartLoc = transform.position;
        TargetsInRange = new List<GameObject>();
        AlreadyHitTargets = new List<GameObject>();
        BounceAmount = projectileStats.BounceNum;
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
                rb.linearVelocity = (target.transform.position - transform.position).normalized * projectileStats.Speed;
                break;
            case ProjectileStats.ProjectileType.Instant:
                rb.position = target.transform.position;
                break;
            case ProjectileStats.ProjectileType.Arcing:
                if (transform.position.y>target.transform.position.y||new Vector3(target.transform.position.x - transform.position.x, 0,
                        target.transform.position.z - transform.position.z).magnitude -
                    new Vector3(target.transform.position.x - StartLoc.x, 0,
                        target.transform.position.z - StartLoc.z).magnitude / 2>0)
                {
                    rb.linearVelocity =
                        new Vector3(target.transform.position.x - transform.position.x, 0,
                            target.transform.position.z - transform.position.z).normalized * projectileStats.Speed +
                        Vector3.up *
                        ((new Vector3(target.transform.position.x - transform.position.x, 0,
                              target.transform.position.z - transform.position.z).magnitude -
                          new Vector3(target.transform.position.x - StartLoc.x, 0,
                              target.transform.position.z - StartLoc.z).magnitude / 2) * projectileStats.ArcHeight* projectileStats.Speed/4);
                }
                else
                {
                    rb.linearVelocity = (target.transform.position - transform.position).normalized * projectileStats.Speed;
                }

                break;
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
        else if(!targetHit)
        {
            MoveToTarget();
        }
    }
    private void OnHit()
    {
        switch (projectileStats.ImpactMode)
        {
            case ProjectileStats.ImpactType.Single:
                DoDamage(target);
                break;
            case ProjectileStats.ImpactType.Aoe:
                foreach (Collider other in Physics.OverlapSphere(transform.position, projectileStats.BounceRange))
                {
                    if (other.gameObject.CompareTag(target.tag))
                    {
                        DoDamage(other.gameObject);
                    }
                }

                GameObject AHEffect=Instantiate(projectileStats.AoeHitEffect, transform.position, Quaternion.identity);
                AHEffect.transform.localScale=Vector3.one*projectileStats.AoeRange;
                break;
            case ProjectileStats.ImpactType.Bouncing:
                DoDamage(target);
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
            StartLoc = target.transform.position;
            target = TargetsInRange.First();
            TargetsInRange = new List<GameObject>();
        }
        else
        {
            BounceAmount = 0;
            StartCoroutine(DestroySelf());
        }

        yield return null;
        GettingTarget = false;
    }

    private void DoDamage(GameObject target)
    {
        AlreadyHitTargets.Add(target);
        if (target.transform.GetComponent<EntityBehaviour>())
        {
            target.transform.GetComponent<EntityBehaviour>().TakeDamage(Damage);
            GameObject targOHE= Instantiate(projectileStats.OnHitEffect,target.transform);
            targOHE.transform.Rotate(Vector3.forward,Random.Range(0,360));
        }
        
    }

    public void DamageDoneTo(float Atk, float AtkMod,GameObject Target)
    {
        Damage = Atk * AtkMod;
        target = Target;
        AlreadyHitTargets = new List<GameObject>();
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(projectileStats.LingeringTime);
        Destroy(gameObject);
    }
}
