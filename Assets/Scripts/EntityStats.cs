using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityStats", menuName = "Entities/EntityStats")]
public class EntityStats : ScriptableObject
{
    public string Name;
    public RangeType Range;
    public float MaxHp;
    public float Atk;
    [Header("How many seconds inbetween attacks")]
    public float AttackInterval; 
    public float Speed;
    public ObjectTag Tag;

    public Ability[] Abilities;
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
    public enum ObjectTag 
    {
        Enemy,
        Tower
    }
}
