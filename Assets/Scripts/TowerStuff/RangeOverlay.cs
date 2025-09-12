using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RangeOverlay : MonoBehaviour
{
    public static RangeOverlay Instance;

    [Header("Appearance")]
    [Tooltip("How many points in the circle (higher = smoother).")]
    public int segments = 64;
    [Tooltip("Y offset so the ring doesn’t Z-fight with the ground.")]
    public float yOffset = 0.05f;
    [Tooltip("Width of the ring line in world units.")]
    public float lineWidth = 0.05f;
    [Tooltip("Optional: set a transparent material on the LineRenderer.")]
    public Material lineMaterial;

    private LineRenderer lr;
    private EntityBehaviour current;
    private float currentRadius;

    void Awake()
    {
        Instance = this;
        lr = GetComponent<LineRenderer>();
        lr.loop = true;
        lr.useWorldSpace = true;
        lr.positionCount = 0;
        lr.enabled = false;

        lr.widthMultiplier = lineWidth;
        if (lineMaterial != null) lr.material = lineMaterial;
    }

    void LateUpdate()
    {
        if (!lr.enabled || current == null) return;
        // follow the target (XZ), keep fixed Y
        UpdateCircle(current.transform.position, currentRadius);
    }

    public void ShowFor(EntityBehaviour tower)
    {
        if (tower == null || tower.Stats == null || tower.Stats.Abilities == null || tower.Stats.Abilities.Length == 0)
        {
            Hide();
            return;
        }

        // Compute radius from abilities (use max ability range)
        float r = tower.Stats.Abilities.Max(a => a.Ability != null ? a.Ability.Range : 0f);
        if (r <= 0f)
        {
            Hide();
            return;
        }

        current = tower;
        currentRadius = r;

        lr.enabled = true;
        lr.positionCount = segments;
        lr.widthMultiplier = lineWidth;

        UpdateCircle(current.transform.position, currentRadius);
    }

    public void Hide()
    {
        lr.enabled = false;
        lr.positionCount = 0;
        current = null;
    }

    public void HideIfTarget(EntityBehaviour e)
    {
        if (current != null && ReferenceEquals(current, e)) Hide();
    }

    private void UpdateCircle(Vector3 center, float radius)
    {
        if (lr.positionCount != segments) lr.positionCount = segments;

        float angleStep = 2f * Mathf.PI / segments;
        float y = center.y + yOffset;

        for (int i = 0; i < segments; i++)
        {
            float angle = i * angleStep;
            float x = center.x + Mathf.Cos(angle) * radius;
            float z = center.z + Mathf.Sin(angle) * radius;
            lr.SetPosition(i, new Vector3(x, y, z));
        }
    }
}
