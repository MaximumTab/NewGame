using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerPopupUI : MonoBehaviour
{
    public TMP_Text statsText;
    private EntityBehaviour currentTarget;
    public static TowerPopupUI Instance;
    public EntityBehaviour CurrentTarget => currentTarget;

    [SerializeField, Range(0f, 1f)] private float sellRefundRate = 0.5f;

    private int guardUntilFrame = 0;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;
        if (Time.frameCount <= guardUntilFrame) return;

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            if (currentTarget != null)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit))
                {
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

    // Hook this to the Sell button's OnClick in the Inspector
    public void OnSellButton()
    {
        Sell(currentTarget);
    }

    public void Sell(EntityBehaviour target)
    {
        if (target == null) return;

        var towerStats = target.Stats as TowerStats;

        // Refund resources (your existing logic) ...
        if (towerStats != null && towerStats.towerCosts != null && ResourceManager.Instance != null)
        {
            foreach (var c in towerStats.towerCosts)
            {
                float refund = Mathf.Max(0f, Mathf.Round(c.resourceCost * 0.5f));
                if (refund > 0f) ResourceManager.Instance.Add(c.resourceType, refund);
            }
        }

        // NEW: give the player the card back into the hand UI
        // If you know which hand slot this tower came from, pass that as siblingIndex.
        DeckLoader.Instance?.AddCardForEntity(target /*, siblingIndex: optional */);

        // Destroy the in-world tower and close popup
        target.DestroySelf();
        Hide();
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
        sb.AppendLine($"Block: {s.Block}");

        if (towerStats != null && towerStats.towerCosts != null && towerStats.towerCosts.Length > 0)
        {
            sb.Append("Cost: ");
            for (int i = 0; i < towerStats.towerCosts.Length; i++)
            {
                var c = towerStats.towerCosts[i];
                sb.Append($"{c.resourceType} {c.resourceCost}");
                if (i < towerStats.towerCosts.Length - 1) sb.Append(", ");
            }

            // Optional: show the 50% sell value line
            sb.AppendLine();
            sb.Append("Sell (50%): ");
            for (int i = 0; i < towerStats.towerCosts.Length; i++)
            {
                var c = towerStats.towerCosts[i];
                var refund = Mathf.Round(c.resourceCost * sellRefundRate);
                sb.Append($"{c.resourceType} {refund}");
                if (i < towerStats.towerCosts.Length - 1) sb.Append(", ");
            }
        }
        return sb.ToString();
    }
}
