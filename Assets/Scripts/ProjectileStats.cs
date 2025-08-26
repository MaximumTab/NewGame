using System;
using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileStats", menuName = "Projectiles/ProjectileStats")]
public class ProjectileStats : ScriptableObject
{
    public ProjectileType DistanceMode;
    [Header ("units per second")]
    public float Speed=5;
    public float ArcHeight=3;
    public ImpactType ImpactMode;
    public float AoeRange=2;
    public float BounceRange = 1;
    public float BounceNum = 0;
    public GameObject AoeHitEffect;
    public GameObject OnHitEffect;
    
    public int NumOfHits = 1;
    public float DelayBtwHit = 0.1f;
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
        Bouncing,
        Single
    }
}
