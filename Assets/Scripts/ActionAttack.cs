using UnityEngine;

[CreateAssetMenu(fileName = "Abilitybase", menuName = "Scriptable Objects/Abilities")]
public class ActionAttack : ScriptableObject
{
    //To be changed according to each enemy;
    public float Range;
    public float AtkModifier;
    public float Cooldown;
    public virtual void UseAbility(GameObject Target, Vector3 Source)
    {
        Debug.Log("You are using the base Ability. The target was "+Target+" from "+ Source);
    }
}
