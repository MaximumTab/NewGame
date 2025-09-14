using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class TowerPopupUI : MonoBehaviour
{
    public TMP_Text statsText;
    private EntityBehaviour currentTarget;
    public static TowerPopupUI Instance;
    public EntityBehaviour CurrentTarget => currentTarget;

    private int guardUntilFrame = 0;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

     void Update()
    {
        if (!gameObject.activeSelf) return;

        // Don’t react to the same frame (or the very next) as the one that opened us
        if (Time.frameCount <= guardUntilFrame) return;

        if (Input.GetMouseButtonDown(0))
        {
            // If click is on UI, don't close
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            // If click is on the same tower, don't close
            if (currentTarget != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
                    // Match exact GO or any parent with the EntityBehaviour of the current target
                    if (hit.collider.gameObject == currentTarget.gameObject ||
                        hit.collider.GetComponentInParent<EntityBehaviour>() == currentTarget)
                        return;
                }
            }

            Hide();
        }
    }

    public void Show(EntityBehaviour target)
    {
        currentTarget = target;
        if (statsText != null) statsText.text = BuildStats(target);
        gameObject.SetActive(true);

        // Guard for the click that opened the popup (open happens on MouseDown)
        guardUntilFrame = Time.frameCount + 1;

        RangeOverlay.Instance?.ShowFor(target);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentTarget = null;

         RangeOverlay.Instance?.Hide();
    }

    public void Refresh()
    {
        if (currentTarget != null && gameObject.activeSelf && statsText != null)
            statsText.text = BuildStats(currentTarget);
    }

    public void RefreshIfTarget(EntityBehaviour e)
    {
        if (gameObject.activeSelf && ReferenceEquals(currentTarget, e))
            Refresh();
    }

    public void HideIfTarget(EntityBehaviour e)
    {
        if (gameObject.activeSelf && ReferenceEquals(currentTarget, e))
            Hide();
    }

    public bool IsShowing(EntityBehaviour e)
    {
        return gameObject.activeSelf && ReferenceEquals(currentTarget, e);
    }

    

    string BuildStats(EntityBehaviour ent)
    {
        if (ent == null || ent.Stats == null) return "No data";
        var s = ent.Stats;
        var towerStats = s as TowerStats;

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine(s.Name);
        sb.AppendLine($"HP: {Mathf.CeilToInt(ent.Hp)} / {Mathf.CeilToInt(s.MaxHp)}");
        sb.AppendLine($"ATK: {s.Atk}");
        sb.AppendLine($"Interval: {s.AttackInterval}s");
        sb.AppendLine($"Blocked: {ent.Blocked}");

        if (towerStats != null && towerStats.towerCosts != null && towerStats.towerCosts.Length > 0)
        {
            sb.Append("Cost: ");
            for (int i = 0; i < towerStats.towerCosts.Length; i++)
            {
                var c = towerStats.towerCosts[i];
                sb.Append($"{c.resourceType} {c.resourceCost}");
                if (i < towerStats.towerCosts.Length - 1) sb.Append(", ");
            }
        }
        return sb.ToString();
    }
}
