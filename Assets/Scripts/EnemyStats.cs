using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Entities/EnemyStats")]
public class EnemyStats : EntityStats
{
    public EnemyType Type;
    public int ObjectiveLives=1;

    public enum EnemyType
    {
        Basic,
        Elite,
        Boss
    }

}
