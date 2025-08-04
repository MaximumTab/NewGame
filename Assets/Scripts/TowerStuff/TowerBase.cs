using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class TowerBase : EntityBehaviour
{
    public TowerStats stats;

    protected Transform target;
    protected float attackCooldown;

    protected virtual void Start()
    {
        OnSpawn();  
    }

    protected virtual void Update()
    {
        attackCooldown -= Time.deltaTime;

        if (target == null)
            return;

        if (attackCooldown <= 0f)
        {
            Attack();
            attackCooldown = stats.attackCooldown;
        }
    }

    protected virtual void Attack()
    {
        if (stats.projectilePrefab != null && target != null)
        {
            GameObject projectile = Instantiate(stats.projectilePrefab, transform.position, Quaternion.identity);
            projectile.GetComponent<TowerBaseProjectile>().Initialize(target, stats.damage);
        }
    }

    

}
