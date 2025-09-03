using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Drag & Drop Settings")]
    public GameObject towerPrefab;    // tower prefab
    public Grid gridRoot;        // the grid its refering to
    public float snapThreshold = 0.5f; // snap to grid range

    [Header("Cost / Placement Rules")]
    public bool requireAffordable = true;   // only allow if we have enough resources

    [Header("Resource Gatherer Placement")]
    public bool isResourceGatherer = false; // tick this for gatherer cards
    public LayerMask resourceLayer;         // layer of Resource tiles
    private float adjacencyRadius = 0.4f;   // how close it must be to count as "next to"

    [Header("Auto-assign Grids By Layer")]
    public LayerMask towerGridLayer;      // layer of the regular tower grid root
    public LayerMask resourceGridLayer;   // layer of the resource grid root

    private Camera mainCamera;
    private CanvasGroup canvasGroup;
    private GameObject draggingTower;
    private TowerBase thisTower;
    private float positionChange;
    private TowerStats.TowerCost[] prefabCosts;
    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text CostText;

    void Awake()
    {
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
        mainCamera = Camera.main;
        SetData();
        
    }

    public void FindAutoGrid()
    {
        if (!gridRoot)
        {
            Grid[] AllGrids = FindObjectsByType<Grid>(FindObjectsSortMode.None);
            foreach (Grid indivGrid in AllGrids)
            {
                if (!isResourceGatherer&&indivGrid.GetComponentInChildren<EnemySpawning>())
                {
                    gridRoot = indivGrid;
                }
                else if(isResourceGatherer)
                {
                    gridRoot = indivGrid;
                }
            }
            
        }
    }
/*
    public void SetGrid()
    {
        if (!gridRoot)
        {
            if (isResourceGatherer)
            {
                GameObject resGrid = GameObject.FindWithTag("ResourceGrid");
                if (resGrid != null)
                {
                    gridRoot = resGrid.transform;
                    Debug.Log($"[CardDrag] Resource grid assigned: {gridRoot.name}");
                }
                else
                {
                    Debug.LogWarning("[CardDrag] Resource grid not found! Please tag your resource grid root 'ResourceGrid'.");
                }
            }
            else
            {
                GameObject normalGrid = GameObject.FindWithTag("TowerGrid");
                if (normalGrid != null)
                {
                    gridRoot = normalGrid.transform;
                    Debug.Log($"[CardDrag] Tower grid assigned: {gridRoot.name}");
                }
                else
                {
                    Debug.LogWarning("[CardDrag] Tower grid not found! Please tag your tower grid root 'TowerGrid'.");
                }
            }
        }
    }*/

    public void SetData()
    {
        thisTower = towerPrefab != null ? towerPrefab.GetComponent<TowerBase>() : null;
        var towerStats = thisTower != null ? thisTower.Stats as TowerStats : null;
        prefabCosts = towerStats != null ? towerStats.towerCosts : null;
        Gatherer gathInfo = towerPrefab.GetComponent<Gatherer>();
        if (NameText&&towerStats)
        {
            NameText.text = towerStats.Name;
            isResourceGatherer = false;
        }else if (NameText&&gathInfo)
        {
            NameText.text = gathInfo.gathererType + " Gatherer";
            isResourceGatherer = true;
        }

        if (CostText&&prefabCosts is { Length: 3 })
        {
            CostText.text = prefabCosts[0].resourceCost + "\n" + prefabCosts[1].resourceCost + "\n" + prefabCosts[2].resourceCost;
        }else if (CostText)
        {
            CostText.text = "Free";
        }
        FindAutoGrid();
        //SetGrid();
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
        snaptoGrid();
    }


    public bool FindDeployableAtLoc(Vector3 Loc)
    {
        positionChange = 0;
        Collider[] ObjectsAtLoc=Physics.OverlapSphere(Loc, 0.4f);
        foreach (Collider col in ObjectsAtLoc)
        {
            Deployable TileDeploy = col.transform.parent.GetComponent<Deployable>();
            if (TileDeploy&&TileDeploy.deployable)
            {
                if (isResourceGatherer&&CanPlaceGathererHere(Loc))
                {
                    positionChange = 0.5f;
                    return true;
                }
                if(isResourceGatherer)
                {
                    return false;
                }

                if (TileDeploy is Path&&thisTower.Stats.Range==EntityStats.RangeType.Melee)
                {
                    positionChange = 0;
                    return true;
                }

                if (TileDeploy is not Path && thisTower.Stats.Range == EntityStats.RangeType.Ranged)
                {
                    positionChange = 0.25f;
                    return true;
                }
                
            }
        }

        return false;
    }

    public void snaptoGrid()
    {
        // snap to nearest grid cell    
        Vector3 cell = new Vector3(Mathf.Floor(draggingTower.transform.position.x),Mathf.RoundToInt(draggingTower.transform.position.y)-1f,Mathf.Floor(draggingTower.transform.position.z))+gridRoot.transform.position+new Vector3(0.5f,0,0.5f);
        if (FindDeployableAtLoc(cell))
        {
            draggingTower.transform.position = cell+Vector3.up*(positionChange);
        }
    }

    public void ReturnCard()
    {
        canvasGroup.alpha = 1f;                  // show card again
        Destroy(draggingTower);                  // remove invalid preview/placement
    }

    public void OnEndDrag(PointerEventData e)
    {
        if (draggingTower == null) return;

        // if this is a resource gatherer, enforce adjacency rule before snapping
        if (!FindDeployableAtLoc(draggingTower.transform.position))
        {
            // invalid placement: restore the card, delete the spawned tower, and exit
            ReturnCard();
            return;
        }
        // final affordability check and spend (handles race conditions)
        if (requireAffordable && prefabCosts != null && ResourceManager.Instance != null)
        {
            if (!ResourceManager.Instance.TrySpend(prefabCosts))
            {
                Debug.LogWarning("[CardDrag] Could not spend resources (now insufficient). Cancelling placement.");
                ReturnCard();
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
        float distance = Vector3.Dot(Vector3.up-mainCamera.transform.position, mainCamera.transform.forward);
        Vector3 sp = new Vector3(screenPos.x, screenPos.y, distance);
        Vector3 wp = mainCamera.ScreenToWorldPoint(sp);
        wp.y = 1;
        return wp;
    }

    // ---- Helpers for gatherer placement ----

    private bool CanPlaceGathererHere(Vector3 pos)
    {
        if (isResourceGatherer)
        {
            // pick mask: if resourceLayer is empty, search all layers
            int mask = (resourceLayer.value != 0) ? resourceLayer.value : ~0;

            Collider[] hits = Physics.OverlapSphere(
                pos,
                adjacencyRadius,
                mask,
                QueryTriggerInteraction.Collide
            );

            if (hits == null || hits.Length == 0)
            {
                Debug.LogWarning($"[CardDrag] No resource colliders in range. " +
                                $"radius={adjacencyRadius}, layerMask={(resourceLayer.value != 0 ? resourceLayer.value : ~0)}");
                return false;
            }

            // get gatherer type from the dragged prefab
            var gatherer = draggingTower.GetComponentInChildren<Gatherer>();
            if (gatherer == null)
            {
                // If no Gatherer script, allow adjacency to ANY ResourceTile
                foreach (var h in hits)
                {
                    // ResourceTile may be on the collider or its parent
                    var tile = h.GetComponent<ResourceTile>() ?? h.GetComponentInParent<ResourceTile>();
                    if (tile != null)
                    {
                        Debug.Log($"[CardDrag] Found resource '{tile.type}' at {h.transform.name}");
                        return true;
                    }
                }
                Debug.LogWarning("[CardDrag] Found colliders, but none had ResourceTile.");
                return false;
            }

            // Require matching resource type
            foreach (var h in hits)
            {
                var tile = h.GetComponent<ResourceTile>() ?? h.GetComponentInParent<ResourceTile>();
                if (tile != null)
                {
                    Debug.Log($"[CardDrag] In range: {tile.type} (need {gatherer.gathererType}) on {h.transform.name}");
                    if (tile.type == gatherer.gathererType)
                        return true;
                }
                else
                {
                    // Helpful debug when layer is right but component missing
                    Debug.Log($"[CardDrag] Collider {h.name} on resource layer but no ResourceTile component.");
                }
            }

            Debug.LogWarning($"[CardDrag] No matching resource tile in range. Need {gatherer.gathererType}.");
            return false;
        }

        // not a resource gatherer -> always OK
        return true;
    }

    

}
