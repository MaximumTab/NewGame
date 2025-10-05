using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "AbilityHeal", menuName = "Abilities/Healing")]
public class HealerAbil : Ability
{
    [Header("SFX Cooldown (seconds)")]
    public float healSFXCooldown = 10f;
    public float lastHealSFXTime = -999f;
    public override void UseAbility(GameObject Target, Vector3 Source, float Atk)
    {
        if (!abilitySFX.IsNull && Time.unscaledTime - lastHealSFXTime >= healSFXCooldown)
        {
            RuntimeManager.PlayOneShot(abilitySFX);
            lastHealSFXTime = Time.unscaledTime;
        }

        GameObject newProj = Instantiate(Projectile, Source, Quaternion.identity);
        if (newProj.GetComponent<ProjectileBehaviour>())
        {
            newProj.GetComponent<ProjectileBehaviour>().DamageDoneTo(-Atk, AtkModifier, Target);
        }
    }
}
