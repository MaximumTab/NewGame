using UnityEngine;

public class HealerAbil : Ability
{
    public override void UseAbility(GameObject Target, Vector3 Source,float Atk)
    {
        GameObject newProj=Instantiate(Projectile, Source, Quaternion.identity);
        if (newProj.GetComponent<ProjectileBehaviour>())
        {
            newProj.GetComponent<ProjectileBehaviour>().DamageDoneTo(-Atk, AtkModifier, Target);
        }
    }
}
