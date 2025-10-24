using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;
    [SerializeField] private Image iconImage; // NEW: optional art for the slot

    [Range(0, 7)] public int slotIndex;

    public int CurrentCardIndex { get; private set; } = -1; // -1=None

    private DeckBuilderDD builder;
    private List<string> labels;

    public void Bind(DeckBuilderDD owner, int index, List<string> optionLabels)
    {
        builder = owner;
        slotIndex = index;
        labels = optionLabels;
        UpdateVisual();
    }

    public void SetCardIndex(int cardIndex)
    {
        CurrentCardIndex = cardIndex; // -1 None, else 0..N-1
        UpdateVisual();
    }

    public int GetPrefValue()
    {
        return CurrentCardIndex >= 0 ? CurrentCardIndex + 1 : 0; // match DeckLoader
    }

    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<CardIconUI>() : null;
        if (icon == null) return;

        var db = DeckBuilderDD.FindActiveDatabase();
        if (db != null && icon.CardIndex >= 0 && icon.CardIndex < db.prerequisiteLevels.Length)
        {
            string prereq = db.prerequisiteLevels[icon.CardIndex];
            if (!string.IsNullOrEmpty(prereq) && !Levels.IsLevelComplete(prereq))
            {
                Debug.Log($"[DeckSlotUI] Cannot assign locked card '{labels[icon.CardIndex + 1]}' (requires {prereq}).");
                return;
            }
        }

        builder.AssignCardToSlot(slotIndex, icon.CardIndex);
    }

    private void UpdateVisual()
    {
        string text = "Empty";
        if (CurrentCardIndex >= 0 && labels != null && CurrentCardIndex + 1 < labels.Count)
            text = labels[CurrentCardIndex + 1];

        if (label) label.text = text;

        if (!iconImage)
            return;

        // Default: show background/placeholder when empty
        if (CurrentCardIndex < 0)
        {
            // Empty slot → show text, keep background visible
            iconImage.sprite = null;          // optional: keep placeholder sprite if you have one
            iconImage.gameObject.SetActive(true);
            if (label) label.gameObject.SetActive(true);
            return;
        }

        // Has a valid card
        var db = DeckBuilderDD.FindActiveDatabase();
        if (db == null || CurrentCardIndex >= db.allCards.Length)
            return;

        var prefab = db.allCards[CurrentCardIndex];
        if (!prefab) return;

        var tb = prefab.GetComponent<TowerBase>();
        var towerStats = tb ? tb.Stats as TowerStats : null;

        if (towerStats != null && towerStats.IconSprite != null)
        {
            iconImage.sprite = towerStats.IconSprite;
            iconImage.gameObject.SetActive(true);
            if (label) label.gameObject.SetActive(false);
        }
        else
        {
            // No art assigned → show text only
            iconImage.sprite = null;
            iconImage.gameObject.SetActive(true); // keep image visible if you want the parchment bg
            if (label) label.gameObject.SetActive(true);
        }
    }
}
