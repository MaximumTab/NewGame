using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerStats", menuName = "TowerDefense/TowerStats")]
public class TowerStats : EntityStats
{

    [Header("Tower cost")]
    public TowerCost[] towerCosts;

    public struct TowerCost
    {
        public enum ResourceType { wood, iron, /* add more recource types herer*/};
        public ResourceType resourceType;

        public int resourceCost;
    }

    //[Header("Visuals")]
    //public Sprite towerSprite;

    // Add other fields as needed
}
