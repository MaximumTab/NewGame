using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardIconUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image iconImage; // optional

    public int CardIndex { get; private set; } // 0..N-1

    private RectTransform rect;
    private Transform originalParent;
    private Vector2 originalAnchoredPos;
    private Canvas rootCanvas;
    private CanvasGroup cg;

    public void Init(int cardIndex, string labelText)
    {
        CardIndex = cardIndex;
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalAnchoredPos = rect.anchoredPosition;

        // Lift to top so it follows the cursor over everything
        if (rootCanvas) transform.SetParent(rootCanvas.transform);
        cg.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rect.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Always snap back to the available panel; the slot records the drop
        transform.SetParent(originalParent);
        rect.anchoredPosition = originalAnchoredPos;
        cg.blocksRaycasts = true;
    }
}
