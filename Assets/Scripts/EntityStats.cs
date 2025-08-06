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
    public SortedBy SortBy;

    public Abil[] Abilities;
    [Flags]
    public enum RangeType
    {
        Melee= 1<<0,
        Ranged= 1<<1
    }
    [Serializable]
    public struct Abil
    {
        public RangeType Range;
        
        public Ability Ability;
    }
    public enum ObjectTag 
    {
        Enemy,
        Tower
    }

    public struct SortedBy
    {
        public Stats Stat;
        public Methods Method;
        public enum Stats
        {
            Hp,
            Atk,
            Order
        }

        public enum Methods
        {
            Greatest,
            None,
            Smallest
        }
    }
}
