using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Entities/TowerStats")]
public class TowerStats : EntityStats
{
    [Header("Tower cost")]
    public TowerCost[] towerCosts;

    [System.Serializable]
    public struct TowerCost
    {
        public ResourceType resourceType;
        public float resourceCost;
    }
    // Add other fields as needed (sprites, DPS, etc.)
}
