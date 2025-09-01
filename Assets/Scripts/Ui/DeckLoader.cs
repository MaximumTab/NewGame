using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class DeckLoader : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private CardDatabase database;   // reference your CardDatabase asset
    [Header("UI")]
    [SerializeField] private Transform deckPanel;     // panel under Canvas, has HorizontalLayoutGroup

    private const string PrefKey = "CardSlot";
    private GameObject AutoGenCard;

    void Start()
    {
        if (database.AutoGenCard)
        {
            AutoGenCard = database.AutoGenCard;
        }

        LoadAndPlaceDeck();
    }

    public void LoadAndPlaceDeck()
    {
        if (database == null || database.allCards.Length == 0)
        {
            Debug.LogError("CardDatabase not assigned or empty.");
            return;
        }

        // Clear out old children (if any)
        foreach (Transform child in deckPanel)
            Destroy(child.gameObject);

        List<GameObject> chosen = new List<GameObject>();

        // Rebuild chosen deck from PlayerPrefs
        for (int i = 0; i < 8; i++) // 8 max slots
        {
            int savedIndex = PlayerPrefs.GetInt($"{PrefKey}{i}", 0);
            if (savedIndex <= 0) continue; // None slot

            int cardIndex = savedIndex - 1; // shift because 0 = None
            if (cardIndex >= 0 && cardIndex < database.allCards.Length)
                chosen.Add(database.allCards[cardIndex]);
        }

        // Instantiate into panel
        foreach (var card in chosen)
        {
            var cardUI = Instantiate(AutoGenCard, deckPanel);
            CardDrag CDUI = cardUI.GetComponent<CardDrag>();
            CDUI.towerPrefab = card;
            CDUI.SetData();
            cardUI.transform.localScale = Vector3.one; // reset scale to avoid stretched UI
        }

        LogDeck(chosen);
    }

    private void LogDeck(List<GameObject> chosen)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[DeckLoader] Loaded {chosen.Count} cards into deck panel:");

        if (chosen.Count == 0)
        {
            sb.AppendLine("  (none)");
        }
        else
        {
            for (int i = 0; i < chosen.Count; i++)
            {
                sb.AppendLine($"  Slot {i}: {chosen[i].name}");
            }
        }

        Debug.Log(sb.ToString());
    }
}
