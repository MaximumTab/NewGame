using UnityEngine;

[CreateAssetMenu(fileName = "Abilitybase", menuName = "Scriptable Objects/Abilities")]
public class ActionAttack : ScriptableObject
{
    //To be changed according to each enemy;
    public float Range;
    public float AtkModifier = 1;
    public int NumOfTargets=1;
    public float Cooldown;
    public GameObject Projectile;
    public virtual void UseAbility(GameObject Target, Vector3 Source,float Atk)
    {
        Debug.Log("You are using the base Ability. The target was "+Target+" from "+ Source);
        GameObject newProj=Instantiate(Projectile, Source, Quaternion.identity);
        if (newProj.GetComponent<ProjectileBehaviour>())
        {
            newProj.GetComponent<ProjectileBehaviour>().DamageDoneTo(Atk, AtkModifier, Target);
        }
    }
}
