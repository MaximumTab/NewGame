using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckSlotUI : MonoBehaviour, IDropHandler
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image background;

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

        builder.AssignCardToSlot(slotIndex, icon.CardIndex);
    }

    private void UpdateVisual()
    {
        string text = "Empty";
        if (CurrentCardIndex >= 0 && labels != null && CurrentCardIndex + 1 < labels.Count)
            text = labels[CurrentCardIndex + 1];

        if (label) label.text = text;

    }
}
