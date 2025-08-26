using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    // index by (int)ResourceType
    [SerializeField] private int[] amounts = new int[3];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Add(ResourceType type, int value)
    {
        amounts[(int)type] += value;
        // TODO: raise UI update event if you have one
    }

    public int Get(ResourceType type) => amounts[(int)type];
}
