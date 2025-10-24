using UnityEngine;

[CreateAssetMenu(fileName = "TowerStats", menuName = "Entities/TowerStats")]
public class TowerStats : EntityStats
{
    [Header("Tower cost")]
    public TowerCost[] towerCosts;

    [Header("Hand Return")]
    public float returnToHandCooldown = 3f; // seconds

    [Header("Tooltip Info")]
    [TextArea(2, 5)]
    public string description;

    [Header("Card Art")]
    public Sprite IconSprite; // NEW — used in deck builder card UI

    [System.Serializable]
    public struct TowerCost
    {
        public ResourceType resourceType;
        public float resourceCost;
    }
    // Add other fields as needed (sprites, DPS, etc.)
}
