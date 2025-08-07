using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag & Drop Settings")]
    public GameObject towerPrefab;    // tower prefab
    public Transform gridRoot;        // the grid its refering to
    public float snapThreshold = 0.5f; // snap to grid range
    
    private Camera mainCamera;       
    private CanvasGroup canvasGroup;
    private GameObject draggingTower;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        mainCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        // hide the card visually but keep it raycast-able
        canvasGroup.alpha = 0f;

        // spawn the tower in world-space
        draggingTower = Instantiate(towerPrefab);
        //--- disable the towers scripts -------
        MonoBehaviour[] scriptsToDisable = draggingTower.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scriptsToDisable)
            script.enabled = false;
        // place under cursor at grid height
        draggingTower.transform.position = ScreenToGridWorldPoint(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        // follow cursor at grid height
        draggingTower.transform.position = ScreenToGridWorldPoint(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        // snap to nearest grid cell
        Transform best = null;
        float bestDist = float.MaxValue;
        foreach (Transform cell in gridRoot)
        {
            float d = Vector3.Distance(cell.position, draggingTower.transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = cell;
            }
        }
        if (best != null && bestDist <= snapThreshold)
            draggingTower.transform.position = best.position;
        //----enable towers scripts from dragingTower -----
        MonoBehaviour[] scriptsToEnable = draggingTower.GetComponentsInChildren<MonoBehaviour>();
        foreach (var script in scriptsToEnable)
            script.enabled = true;
        // remove the card UI
        Destroy(gameObject);
    }

    // Projects screen point into world at the Y-level of gridRoot
    private Vector3 ScreenToGridWorldPoint(Vector2 screenPos)
    {
        // calculate distance along camera forward to grid plane
        float distance = Vector3.Dot(Vector3.up * gridRoot.position.y - mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 sp = new Vector3(screenPos.x, screenPos.y, distance);
        Vector3 wp = mainCamera.ScreenToWorldPoint(sp);
        wp.y = gridRoot.position.y;
        return wp;
    }
}
