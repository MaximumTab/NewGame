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
        BoxCol.isTrigger = true;
        base.OnSpawn();
        
        //BoxCol.center.Set(0,0.2f,0);
    }

    private void Start()
    {
        OnSpawn();
    }
}
