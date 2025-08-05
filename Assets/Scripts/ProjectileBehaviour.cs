using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ProjectileBehaviour : MonoBehaviour
{
    public ProjectileStats projectileStats;
    private GameObject target;
    private float Damage;
    private bool targetHit = false;
    private Vector3 StartLoc;

    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        StartLoc = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target)
        {
            CheckOnHit();
        }
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
            OnHit();
            StartCoroutine(DestroySelf());
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
                SphereCollider SphCol = gameObject.AddComponent<SphereCollider>();
                SphCol.radius = projectileStats.AoeRange;
                SphCol.isTrigger = true;
                GameObject AHEffect=Instantiate(projectileStats.AoeHitEffect, transform.position, Quaternion.identity);
                AHEffect.transform.localScale=Vector3.one*projectileStats.AoeRange;
                break;
        }
    }

    private void DoDamage(GameObject target)
    {
        if (target.transform.GetComponent<EntityBehaviour>())
        {
            target.transform.GetComponent<EntityBehaviour>().TakeDamage(Damage);
            Instantiate(projectileStats.OnHitEffect,target.transform);
        }
    }

    public void DamageDoneTo(float Atk, float AtkMod,GameObject Target)
    {
        Damage = Atk * AtkMod;
        target = Target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(target.tag))
        {
            DoDamage(other.gameObject);
        }
    }

    IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(projectileStats.LingeringTime);
        Destroy(gameObject);
    }
}
