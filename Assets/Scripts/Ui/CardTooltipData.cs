using UnityEngine;

[System.Serializable]
public class CardTooltipData
{
    public string DisplayName;
    public string Description;
    public string CostInfo;
    public string UnlockInfo;
    public bool IsLocked;

    public CardTooltipData(string displayName)
    {
        DisplayName = displayName;
    }
}
