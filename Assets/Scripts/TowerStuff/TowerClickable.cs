using UnityEngine;
using UnityEngine.EventSystems;

public class TowerClickable : MonoBehaviour
{
    private TowerPopupUI popup;

    void Start()
    {
        // Find the popup under GameManager/Canvas
        var canvas = GameObject.Find("Canvas");
        if (canvas != null)
            popup = canvas.GetComponentInChildren<TowerPopupUI>(true);
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;
        if (popup == null) return;

        var eb = GetComponentInParent<EntityBehaviour>();
        if (eb == null) return;

        // If the popup is already showing THIS tower, hide it.
        // Otherwise always show/replace with the new tower.
        if (popup.IsShowing(eb))
        {
            popup.Hide();
        }
        else
        {
            popup.Show(eb);
        }
    }

    void OnDisable()
    {
        if (popup != null)
            popup.HideIfTarget(GetComponentInParent<EntityBehaviour>());
    }
}
