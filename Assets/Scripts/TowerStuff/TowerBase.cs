using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]
public class TowerBase : EntityBehaviour
{

    protected virtual void Start()
    {
        OnSpawn();  
    }


}
