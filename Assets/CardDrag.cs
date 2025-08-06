using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rect;
    Canvas canvas;
    private Vector2 originalPos;
    private Transform originalParent;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalPos = rect.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        transform.localScale = Vector3.one * 0.9f;
    }

    public void OnDrag(PointerEventData e)
    {
        rect.anchoredPosition += e.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
       
        {
            transform.SetParent(originalParent);
            rect.anchoredPosition = originalPos;
        }

        transform.localScale = Vector3.one;
    }

}
