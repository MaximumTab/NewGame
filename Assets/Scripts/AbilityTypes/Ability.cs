using UnityEngine;

[CreateAssetMenu(fileName = "AbilityProjectile", menuName = "Abilities/Projectiles")]
public class Ability : ScriptableObject
{
    //To be changed according to each enemy;
    public float Range;
    public float AtkModifier = 1;
    public int NumOfTargets=1;
    public float Cooldown;
    public GameObject Projectile;
    public virtual void UseAbility(GameObject Target, Vector3 Source,float Atk)
    {
        GameObject newProj=Instantiate(Projectile, Source, Quaternion.identity);
        if (newProj.GetComponent<ProjectileBehaviour>())
        {
            newProj.GetComponent<ProjectileBehaviour>().DamageDoneTo(Atk, AtkModifier, Target);
        }
    }
}
