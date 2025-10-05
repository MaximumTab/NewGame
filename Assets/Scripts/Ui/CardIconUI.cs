using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage; // optional

    public int CardIndex { get; private set; } // 0..N-1

    private RectTransform rect;
    private Transform originalParent;
    private Vector2 originalAnchoredPos;
    private Canvas rootCanvas;
    private CanvasGroup cg;
    private GameObject cardPrefab;
    public void Init(int cardIndex, string labelText, GameObject prefab = null)
    {
        CardIndex = cardIndex;
        cardPrefab = prefab;
        if (!rect) rect = GetComponent<RectTransform>();
        if (nameText) nameText.text = labelText;
    }

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        cg = GetComponent<CanvasGroup>();
        if (!cg) cg = gameObject.AddComponent<CanvasGroup>(); // needed for proper dropping
        rootCanvas = GetComponentInParent<Canvas>();
        if (!rootCanvas) Debug.LogWarning("[CardIconUI] No Canvas found in parents.");
    }

    private bool IsLocked()
    {
        var db = DeckBuilderDD.FindActiveDatabase();
        if (db == null || CardIndex < 0 || CardIndex >= db.prerequisiteLevels.Length)
            return false;

        string prereq = db.prerequisiteLevels[CardIndex];
        if (!string.IsNullOrEmpty(prereq) && !Levels.IsLevelComplete(prereq))
            return true;

        return false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CardTooltip.Instance == null) return;

        string displayName = "Unknown";

        if (cardPrefab != null)
        {
            var ent = cardPrefab.GetComponent<EntityBehaviour>();
            if (ent && ent.Stats != null && !string.IsNullOrEmpty(ent.Stats.Name))
            {
                displayName = ent.Stats.Name;
            }
            else
            {
                // fallback to prefab name (clean up "(Clone)" if needed)
                displayName = cardPrefab.name.Replace("(Clone)", "").Trim();
            }
        }

        CardTooltipData data = new CardTooltipData(displayName);

        bool locked = IsLocked();
        data.IsLocked = locked;

        // Description & cost
        if (cardPrefab != null)
        {
            var tb = cardPrefab.GetComponent<TowerBase>();
            var stats = tb ? tb.Stats as TowerStats : null;

            if (tb)
                data.Description = $"Tower Type: {tb.GetType().Name}";

            if (stats && stats.towerCosts != null)
            {
                foreach (var c in stats.towerCosts)
                    data.CostInfo += $"{c.resourceType}: {c.resourceCost}\n";
            }

            if (!string.IsNullOrEmpty(data.CostInfo))
                data.CostInfo = data.CostInfo.Trim();
        }

        // Unlock info
        if (locked)
        {
            string prereq = GetPrerequisiteName();
            data.UnlockInfo = $"Unlock by completing: {prereq}";
        }

        CardTooltip.Instance.Show(data);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (CardTooltip.Instance) CardTooltip.Instance.Hide();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (IsLocked()) return;

        originalParent = transform.parent;
        originalAnchoredPos = rect.anchoredPosition;

        if (rootCanvas) transform.SetParent(rootCanvas.transform);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (IsLocked()) return;
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (IsLocked()) return;

        transform.SetParent(originalParent);
        rect.anchoredPosition = originalAnchoredPos;
        cg.blocksRaycasts = true;
    }
    
    private string GetPrerequisiteName()
    {
        var db = DeckBuilderDD.FindActiveDatabase();
        if (db == null || CardIndex < 0 || CardIndex >= db.prerequisiteLevels.Length)
            return "";
        return db.prerequisiteLevels[CardIndex];
    }
}
