using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Entities/EnemyStats")]
public class EnemyStats : EntityStats
{
    public EnemyType Type;

    public enum EnemyType
    {
        Basic,
        Elite,
        Boss
    }

}
