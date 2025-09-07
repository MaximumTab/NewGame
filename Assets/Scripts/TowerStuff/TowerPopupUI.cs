using TMPro;
using UnityEngine;

public class TowerPopupUI : MonoBehaviour
{
    public TMP_Text statsText;
    private EntityBehaviour currentTarget;
    public static TowerPopupUI Instance;

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(EntityBehaviour target)
    {
        currentTarget = target;
        if (statsText != null) statsText.text = BuildStats(target);
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        currentTarget = null;
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
