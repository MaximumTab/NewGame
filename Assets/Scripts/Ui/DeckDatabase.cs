using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Game/Card Database")]
public class CardDatabase : ScriptableObject
{
    public GameObject AutoGenCard;
    public GameObject[] allCards;
    
    [Tooltip("Optional: name of level required to unlock each card (index matches allCards)")]
    public string[] prerequisiteLevels;
}
