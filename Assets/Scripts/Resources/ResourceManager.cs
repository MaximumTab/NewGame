using System;
using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private TMP_Text ResourceShow;

    // index by (int)ResourceType: wood, stone, iron
    [SerializeField] private float[] amounts = new float[3];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Optional: DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (ResourceShow)
        {
            ResourceShow.text = "Resources:\nWood: " + amounts[0] + "\nStone: " + amounts[1] + "\nIron: " + amounts[2];
        }
    }

    public float Get(ResourceType type) => amounts[(int)type];

    public void Add(ResourceType type, float value)
    {
        amounts[(int)type] += value;
        // TODO: notify UI
    }

    public bool CanAfford(TowerStats.TowerCost[] costs)
    {
        if (costs == null) return true;
        for (int i = 0; i < costs.Length; i++)
        {
            if (Get(costs[i].resourceType) < costs[i].resourceCost)
                return false;
        }
        return true;
    }

    // Atomically spend, false if not enough
    public bool TrySpend(TowerStats.TowerCost[] costs)
    {
        if (!CanAfford(costs)) return false;
        for (int i = 0; i < costs.Length; i++)
        {
            amounts[(int)costs[i].resourceType] -= costs[i].resourceCost;
        }
        // TODO: notify UI
        return true;
    }
}
