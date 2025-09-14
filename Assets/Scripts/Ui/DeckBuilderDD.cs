using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckBuilderDD : MonoBehaviour
{
    [Header("Database")]
    [SerializeField] private CardDatabase database;

    [Header("UI")]
    [SerializeField] private RectTransform availablePanel;
    [SerializeField] private DeckSlotUI[] slots; // size 8
    [SerializeField] private GameObject cardIconPrefab; // has CardIconUI

    private const string PrefKey = "CardSlot";
    private const string NoneLabel = "— None —";

    // index -> label; 0 is None, 1..N map to database.allCards (index+1)
    private List<string> optionLabels = new();

    private void Start()
    {
        if (database == null || database.allCards == null || database.allCards.Length == 0)
        {
            Debug.LogError("[DeckBuilderDD] CardDatabase not assigned or empty.");
            return;
        }

        BuildLabelList();     // fills optionLabels incl. None at 0
        BuildAvailableList(); // make draggable icons for all cards
        RestoreFromPrefs();   // fill slots from PlayerPrefs
        LogDeck();
    }

    private void BuildLabelList()
    {
        optionLabels.Clear();
        optionLabels.Add(NoneLabel); // 0 = None

        foreach (var prefab in database.allCards)
        {
            if (prefab == null) { optionLabels.Add("(Missing)"); continue; }

            // Try to get a nice display name like your old script did
            string label = prefab.name;

            var ent = prefab.GetComponent<EntityBehaviour>();
            if (ent && ent.Stats != null && !string.IsNullOrEmpty(ent.Stats.Name))
                label = ent.Stats.Name;
            else
            {
                var g = prefab.GetComponent<Gatherer>();
                if (g != null) label = g.gathererType + " Gatherer";
            }

            optionLabels.Add(label);
        }
    }

    private void BuildAvailableList()
    {
        foreach (Transform c in availablePanel) Destroy(c.gameObject);

        // for each entry in database (labels index 1..N), create an icon you can drag
        for (int i = 0; i < database.allCards.Length; i++)
        {
            var go = Instantiate(cardIconPrefab, availablePanel);
            var ui = go.GetComponent<CardIconUI>();
            if (!ui)
            {
                Debug.LogError("[DeckBuilderDD] cardIconPrefab must have CardIconUI.");
                continue;
            }

            string label = optionLabels[i + 1]; // shift because 0 = None
            ui.Init(cardIndex: i, labelText: label);
        }
    }

    private void RestoreFromPrefs()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var slot = slots[i];
            if (!slot) continue;

            slot.Bind(this, i, optionLabels);

            int saved = PlayerPrefs.GetInt($"{PrefKey}{i}", 0); // 0=None
            int cardIndex = saved - 1; // -1=None, 0..N-1 = database index
            slot.SetCardIndex(cardIndex);
        }
    }

    // Called by DeckSlotUI when a drop happens
    public void AssignCardToSlot(int slotIndex, int cardIndex)
    {
        if (slotIndex < 0 || slotIndex >= slots.Length) return;
        slots[slotIndex].SetCardIndex(cardIndex);
    }

    public void SaveSelection()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int v = slots[i] ? slots[i].GetPrefValue() : 0; // 0=None, else index+1
            PlayerPrefs.SetInt($"{PrefKey}{i}", v);
        }
        PlayerPrefs.Save();
        Debug.Log("[DeckBuilderDD] Deck saved.");
        LogDeck();
    }

    public void ClearDeck()
    {
        foreach (var s in slots) if (s) s.SetCardIndex(-1);
        for (int i = 0; i < slots.Length; i++) PlayerPrefs.SetInt($"{PrefKey}{i}", 0);
        PlayerPrefs.Save();
        Debug.Log("[DeckBuilderDD] Deck cleared.");
        LogDeck();
    }

    public void LogDeck()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[DeckBuilderDD] Current deck:");
        for (int i = 0; i < slots.Length; i++)
        {
            string label = NoneLabel;
            int idx = slots[i] ? slots[i].CurrentCardIndex : -1;
            if (idx >= 0 && idx + 1 < optionLabels.Count) label = optionLabels[idx + 1];
            sb.AppendLine($"  Slot {i}: {label}");
        }
        Debug.Log(sb.ToString());
    }
}
