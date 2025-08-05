using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats", menuName = "Projectiles/ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    public ProjectileType DistanceMode;
    [Header ("units per second")]
    public float Speed;
    public float ArcHeight;
    public ImpactType ImpactMode;
    public float AoeRange;
    public GameObject AoeHitEffect;
    public GameObject OnHitEffect;
    public float LingeringTime=0.1f;
    
    public enum ProjectileType
    {
        Moving,
        Instant,
        Arcing
    }
    public enum ImpactType
    {
        Aoe,
        Single
    }
}
