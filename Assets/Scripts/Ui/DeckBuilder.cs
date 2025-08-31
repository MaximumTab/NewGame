using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public class DeckBuilder : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Dropdown[] dropdowns;   // assign up to 8 in inspector
    [Header("Database")]
    [SerializeField] private CardDatabase database;      // reference your CardDatabase asset

    private const string PrefKey = "CardSlot";
    private const string NoneLabel = "— None —";

    void Start()
    {
        if (database == null || database.allCards.Length == 0)
        {
            Debug.LogError("CardDatabase not assigned or empty.");
            return;
        }

        // Build option list (0 = None)
        var options = new List<string> { NoneLabel };
        foreach (var card in database.allCards)
        {
            if (card != null) options.Add(card.name);
        }

        // Populate dropdowns and restore saved values
        for (int i = 0; i < dropdowns.Length; i++)
        {
            var dd = dropdowns[i];
            if (dd == null) continue;

            dd.ClearOptions();
            dd.AddOptions(options);

            int saved = PlayerPrefs.GetInt($"{PrefKey}{i}", 0);
            dd.value = Mathf.Clamp(saved, 0, options.Count - 1);

            int slot = i; // capture for closure
            dd.onValueChanged.AddListener(v =>
            {
                Debug.Log($"[DeckBuilder] Slot {slot} set to {(v == 0 ? NoneLabel : options[v])}");
            });
        }

        LogDeck();
    }

    // Save deck to PlayerPrefs
    public void SaveSelection()
    {
        for (int i = 0; i < dropdowns.Length; i++)
        {
            var dd = dropdowns[i];
            if (dd == null) continue;

            PlayerPrefs.SetInt($"{PrefKey}{i}", dd.value);
        }
        PlayerPrefs.Save();
        Debug.Log("[DeckBuilder] Deck saved.");
        LogDeck();
    }

    // Clear all slots to None
    public void ClearDeck()
    {
        for (int i = 0; i < dropdowns.Length; i++)
        {
            var dd = dropdowns[i];
            if (dd == null) continue;

            dd.value = 0; // None
            PlayerPrefs.SetInt($"{PrefKey}{i}", 0);
        }
        PlayerPrefs.Save();
        Debug.Log("[DeckBuilder] Deck cleared.");
        LogDeck();
    }

    // Returns selected prefabs (ignores None, duplicates allowed)
    public GameObject[] GetSelectedCards()
    {
        var chosen = new List<GameObject>();

        for (int i = 0; i < dropdowns.Length; i++)
        {
            int v = dropdowns[i]?.value ?? 0;
            if (v <= 0) continue; // None

            int cardIndex = v - 1; // shift because 0 = None
            if (cardIndex >= 0 && cardIndex < database.allCards.Length)
                chosen.Add(database.allCards[cardIndex]);
        }

        return chosen.ToArray();
    }

    // Debug log of current deck
    public void LogDeck()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[DeckBuilder] Current deck:");

        for (int i = 0; i < dropdowns.Length; i++)
        {
            var dd = dropdowns[i];
            if (dd == null)
            {
                sb.AppendLine($"  Slot {i}: (missing dropdown)");
                continue;
            }
            string label = dd.options[dd.value].text;
            sb.AppendLine($"  Slot {i}: {label}");
        }

        var actual = GetSelectedCards();
        sb.Append("  Selected (compact): ");
        sb.Append(actual.Length == 0 ? "none" : string.Join(", ", System.Array.ConvertAll(actual, c => c.name)));

        Debug.Log(sb.ToString());
    }
}
