using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Scriptable Objects/EnemyStats")]
public class EnemyStats : ScriptableObject
{
    public string Name;
    public EnemyType Type;
    public RangeType Range;
    public float MaxHp;
    public float Atk;
    public float AttackInterval;
    public float Speed;

    public Ability[] Abilities;
    public enum EnemyType
    {
        Basic,
        Elite,
        Boss
    }
    [Flags]
    public enum RangeType
    {
        Melee= 1<<0,
        Ranged= 1<<1
    }
    [Serializable]
    public struct Ability
    {
        public RangeType Range;
        
        public ActionAttack AtkExecute;
    }

}
