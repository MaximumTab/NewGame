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
    public static DeckLoader Instance { get; private set; }

     void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

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

     // ---------- NEW API: spawn a card icon back into the hand ----------
    public GameObject AddCardToHand(GameObject towerPrefab, int siblingIndex = -1)
    {
        if (!towerPrefab) return null;
        if (!AutoGenCard && database) AutoGenCard = database.AutoGenCard;
        if (!AutoGenCard)
        {
            Debug.LogError("DeckLoader: AutoGenCard prefab not set.");
            return null;
        }

        var cardUI = Instantiate(AutoGenCard, deckPanel);
        if (siblingIndex >= 0 && siblingIndex <= deckPanel.childCount - 1)
            cardUI.transform.SetSiblingIndex(siblingIndex);

        var cd = cardUI.GetComponent<CardDrag>();
        cd.towerPrefab = towerPrefab;
        cd.SetData();
        cardUI.transform.localScale = Vector3.one;
        return cardUI;
    }

    public void AddCardToHandFromBattle(EntityBehaviour towerPrefab,float CooldownSeconds)
    {
        CardCooldown cd = AddCardForEntity(towerPrefab).GetComponent<CardCooldown>();
        if (cd) cd.BeginCooldown(CooldownSeconds);
    }

    // Convenience: resolve the prefab by looking up the entity’s tower type in the DB
    public GameObject AddCardForEntity(EntityBehaviour soldEntity, int siblingIndex = -1)
    {
        if (soldEntity == null || database == null) return null;

        // Match by TowerBase/Stats.Name. Adjust this if you have a better ID.
        var soldBase = soldEntity.GetComponent<TowerBase>();
        var soldName = soldBase && soldBase.Stats ? soldBase.Stats.Name : soldEntity.Stats?.Name;
        if (string.IsNullOrEmpty(soldName))
        {
            Debug.LogWarning("DeckLoader: could not resolve sold tower name.");
            return null;
        }

        foreach (var prefab in database.allCards)
        {
            var tb = prefab ? prefab.GetComponent<TowerBase>() : null;
            if (tb && tb.Stats && tb.Stats.Name == soldName)
                return AddCardToHand(prefab, siblingIndex);
        }

        Debug.LogWarning($"DeckLoader: no prefab found in database for '{soldName}'.");
        return null;
    }
    // ------------------------------------------------------------------

    private void CreateCardIcon(GameObject towerPrefab)
    {
        var cardUI = Instantiate(AutoGenCard, deckPanel);
        var cd = cardUI.GetComponent<CardDrag>();
        cd.towerPrefab = towerPrefab;
        cd.SetData();
        cardUI.transform.localScale = Vector3.one;
    }
}
