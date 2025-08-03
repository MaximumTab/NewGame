using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats", menuName = "Scriptable Objects/ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    public ProjectileType DistanceMode;
    public float Speed;
    public ImpactType ImpactMode;
    public float AoeRange;
    public GameObject AoeHitEffect;
    public GameObject OnHitEffect;
    public float LingeringTime=0.1f;
    
    public enum ProjectileType
    {
        Arc,
        Instant
    }
    public enum ImpactType
    {
        Aoe,
        Single
    }
}
