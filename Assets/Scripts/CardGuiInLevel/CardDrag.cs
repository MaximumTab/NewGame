using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag & Drop Settings")]
    public GameObject towerPrefab;    // tower prefab
    public Transform gridRoot;        // the grid its refering to
    public float snapThreshold = 0.5f; // snap to grid range

    [Header("Resource Gatherer Placement")]
    public bool isResourceGatherer = false;        // tick this for gatherer cards
    public LayerMask resourceLayer;                // layer of your Resource tiles
    public float adjacencyRadius = 1.25f;          // how close it must be to count as "next to"

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
        // if this is a resource gatherer, enforce adjacency rule before snapping
        if (isResourceGatherer && !CanPlaceGathererHere(draggingTower.transform.position))
        {
            // invalid placement: restore the card, delete the spawned tower, and exit
            canvasGroup.alpha = 1f;                  // show card again
            transform.localScale = Vector3.one;      // reset scale in case you shrink elsewhere
            Destroy(draggingTower);                  // remove invalid preview/placement
            return;
        }

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

    // ---- Helpers for gatherer placement ----

    // Checks if there is a matching resource tile within adjacencyRadius.
    // If the spawned prefab has a Gatherer component, we match its type.
    // If it doesn't, we just require any ResourceTile in range.
    private bool CanPlaceGathererHere(Vector3 pos)
    {
        // collect nearby colliders on resource layer
        Collider[] hits = Physics.OverlapSphere(pos, adjacencyRadius, resourceLayer);

        if (hits == null || hits.Length == 0)
            return false;

        // try to read gatherer type from the spawned tower (if present)
        var gatherer = draggingTower.GetComponentInChildren<Gatherer>(); // your gatherer script with a ResourceType field
        if (gatherer == null)
        {
            // no gatherer script found -> allow placement next to any resource tile
            // (you can change this to 'return false' if you want to hard-require Gatherer)
            for (int i = 0; i < hits.Length; i++)
                if (hits[i].GetComponent<ResourceTile>() != null)
                    return true;
            return false;
        }

        // match by resource type
        ResourceType needType = gatherer.gathererType;
        for (int i = 0; i < hits.Length; i++)
        {
            var tile = hits[i].GetComponent<ResourceTile>();
            if (tile != null && tile.type == needType)
                return true;
        }
        return false;
    }
}
