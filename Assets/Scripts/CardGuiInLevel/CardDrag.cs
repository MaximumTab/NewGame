using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag & Drop Settings")]
    public GameObject towerPrefab;    // tower prefab
    public Transform gridRoot;        // the grid its refering to
    public float snapThreshold = 0.5f; // snap to grid range

    [Header("Cost / Placement Rules")]
    public bool requireAffordable = true;   // only allow if we have enough resources

    [Header("Resource Gatherer Placement")]
    public bool isResourceGatherer = false; // tick this for gatherer cards
    public LayerMask resourceLayer;         // layer of Resource tiles
    public float adjacencyRadius = 1.25f;   // how close it must be to count as "next to"

    private Camera mainCamera;       
    private CanvasGroup canvasGroup;
    private GameObject draggingTower;
    private TowerStats.TowerCost[] prefabCosts;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        mainCamera = Camera.main;

        // pull TowerStats from the prefab’s EntityBehaviour
        var eb = towerPrefab != null ? towerPrefab.GetComponent<EntityBehaviour>() : null;
        var towerStats = eb != null ? eb.Stats as TowerStats : null;
        prefabCosts = towerStats != null ? towerStats.towerCosts : null;
        // Auto-assign gridRoot if not set
        if (gridRoot == null)
        {
            GameObject found = GameObject.Find("Grid");
            if (found != null)
                gridRoot = found.transform;
            else
                Debug.LogWarning("GridRoot not found in scene. Please assign gridRoot manually.");
        }
    }

    public void OnBeginDrag(PointerEventData e)
    {
        // affordability gate before spawning preview
        if (requireAffordable && prefabCosts != null && ResourceManager.Instance != null)
        {
            if (!ResourceManager.Instance.CanAfford(prefabCosts))
            {
                Debug.LogWarning("[CardDrag] Not enough resources to place this tower.");
                return; // do not start dragging, keep the card visible
            }
        }

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
        if (draggingTower == null) return;
        // follow cursor at grid height
        draggingTower.transform.position = ScreenToGridWorldPoint(e.position);
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (draggingTower == null) return;

        // if this is a resource gatherer, enforce adjacency rule before snapping
        if (isResourceGatherer && !CanPlaceGathererHere(draggingTower.transform.position))
        {
            // invalid placement: restore the card, delete the spawned tower, and exit
            canvasGroup.alpha = 1f;                  // show card again
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

        // final affordability check and spend (handles race conditions)
        if (requireAffordable && prefabCosts != null && ResourceManager.Instance != null)
        {
            if (!ResourceManager.Instance.TrySpend(prefabCosts))
            {
                Debug.LogWarning("[CardDrag] Could not spend resources (now insufficient). Cancelling placement.");
                Destroy(draggingTower);          // cancel placement
                canvasGroup.alpha = 1f;          // show card again
                return;
            }
        }

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

    private bool CanPlaceGathererHere(Vector3 pos)
    {
        // find nearby resource tiles on the specified layer
        var hits = Physics.OverlapSphere(pos, adjacencyRadius, resourceLayer);
        if (hits == null || hits.Length == 0) return false;

        // if the prefab has a Gatherer script, match tile type to gatherer type
        var gatherer = draggingTower.GetComponentInChildren<Gatherer>();
        if (gatherer != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                var tile = hits[i].GetComponent<ResourceTile>();
                if (tile != null && tile.type == gatherer.gathererType)
                    return true;
            }
            return false;
        }

        // otherwise, allow any resource tile adjacency
        for (int i = 0; i < hits.Length; i++)
            if (hits[i].GetComponent<ResourceTile>() != null)
                return true;

        return false;
    }
}
