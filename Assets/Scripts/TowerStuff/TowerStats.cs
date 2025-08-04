using UnityEngine;

[CreateAssetMenu(fileName = "NewTowerStats", menuName = "TowerDefense/TowerStats")]
public class TowerStats : ScriptableObject
{
    [Header("Attack Stats")]
    public float damage;
    public float range;
    public float attackCooldown;  

    [Header("Economy Stats")]
    public int cost;
    public int upgradeCost;

    [Header("Projectile")]
    public GameObject projectilePrefab;

    //[Header("Visuals")]
    //public Sprite towerSprite;

    // Add other fields as needed
}
