using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Scriptable Objects/TowerStats")]
public class TowerStats : EntityStats
{
    public TowerType Type;
    
    public enum TowerType
    {
        Melee,
        Ranged,
        Resource
    }
}
