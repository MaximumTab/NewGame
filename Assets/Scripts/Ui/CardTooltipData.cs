using UnityEngine;

[System.Serializable]
public class CardTooltipData
{
    public string DisplayName;
    public string Description;
    public string CostInfo;
    public string UnlockInfo;
    public bool IsLocked;

    // NEW FIELDS
    public string MaxHP;
    public string Damage;
    public string Range;

    public CardTooltipData(string displayName)
    {
        DisplayName = displayName;
    }
}
