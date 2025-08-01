using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Scriptable Objects/TowerStats")]
public class TowerStats : ScriptableObject
{
    
    public string Name;
    public TowerType Type;
    public float Range;
    public float MaxHp;
    public float Atk;
    public float AttackInterval;
    
    public enum TowerType
    {
        Melee,
        Ranged,
        Resource
    }
}
