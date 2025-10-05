using UnityEngine;

[System.Serializable]
public class CardTooltipData
{
    public string DisplayName;
    public string Description;
    public string CostInfo;
    public string UnlockInfo;
    public bool IsLocked;

    public string MaxHP;
    public string Damage;
    public string Range;

    // NEW FIELD
    public Sprite TowerSprite;

    public CardTooltipData(string displayName)
    {
        DisplayName = displayName;
    }
}
