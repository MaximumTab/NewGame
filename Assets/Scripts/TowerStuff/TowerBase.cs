using System;
using UnityEngine;


public class TowerBase : EntityBehaviour
{
    public override void OnSpawn()
    {
        rb = gameObject.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        BoxCollider BoxCol=gameObject.AddComponent<BoxCollider>();
        BoxCol.size = new Vector3(0.7f, 1, 0.7f);
        BoxCol.center= Vector3.up*0.5f;
        BoxCol.isTrigger = true;
        base.OnSpawn();
        
        //BoxCol.center.Set(0,0.2f,0);
    }

    private void Start()
    {
        OnSpawn();
    }

    public override void CheckAlive()
    {
        if (Hp <= 0)
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
            
        }
        PercHp = Hp / MaxHp;
        if(HpSlider)
            HpSlider.value = PercHp;
    }
}
