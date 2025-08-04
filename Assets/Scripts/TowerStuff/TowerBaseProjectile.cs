using UnityEngine;

public class TowerBaseProjectile : MonoBehaviour
{
    protected Transform target;
    protected float damage;
    public float speed = 5f;

    public virtual void Initialize(Transform target, float damage)
    {
        this.target = target;
        this.damage = damage;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            HitTarget();
        }
    }

    protected virtual void HitTarget()
    {
        if (target.CompareTag("Enemy"))
        {
            // Implement your generic logic here, e.g., Destroy, deal damage, etc.
            // Example: Destroy the enemy GameObject directly:
            Destroy(target.gameObject);
        }

        Destroy(gameObject);
    }
}
