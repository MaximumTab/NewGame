using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gatherer : MonoBehaviour
{
    [Header("Gatherer Setup")]
    public ResourceType gathererType = ResourceType.Wood;
    [Min(1)] public float perSecond = 1;
    [Min(0.05f)] public float tickSeconds = 1f;

    [Header("Placement Rules")]
    public float adjacencyRadius = 1.25f;
    public LayerMask resourceLayer;

    [Header("Debug")]
    public bool debugOnPlacement = true;
    public float debugDrawSeconds = 2f;

    private Coroutine loop;

    private readonly List<Transform> _debugTargets = new List<Transform>();
    private float _debugUntil = 0f;

    void OnEnable()
    {
        if (!IsNextToCorrectResource(out var hits))
        {
            Debug.LogWarning($"[Gatherer] '{name}' not adjacent to required resource ({gathererType}). Disabling.");
            enabled = false;
            return;
        }

        // Debug: list resources in range + draw lines for a bit
        if (debugOnPlacement)
        {
            _debugTargets.Clear();
            for (int i = 0; i < hits.Length; i++)
            {
                var tile = hits[i].GetComponent<ResourceTile>();
                if (tile != null && tile.type == gathererType)
                {
                    _debugTargets.Add(hits[i].transform);
                    Debug.Log($"[Gatherer] In range: {tile.type} tile '{hits[i].name}' @ {hits[i].transform.position}");
                }
            }
            _debugUntil = Time.time + debugDrawSeconds;
            Debug.Log($"[Gatherer] {gathererType}: { _debugTargets.Count } matching resource tile(s) in range (r={adjacencyRadius}).");
        }

        loop = StartCoroutine(TickLoop());
    }

    void OnDisable()
    {
        if (loop != null) StopCoroutine(loop);
    }

    void Update()
    {
        // Draw green lines to each matching resource tile for a short time after placement
        if (Time.time < _debugUntil)
        {
            for (int i = 0; i < _debugTargets.Count; i++)
            {
                Transform t = _debugTargets[i];
                if (t != null)
                {
                    Debug.DrawLine(transform.position + Vector3.up * 0.2f,
                                   t.position + Vector3.up * 0.2f,
                                   Color.green, 0f, false);
                }
            }
            // also show the adjacency radius as a simple crosshair circle approximation
            DebugDrawCircle(transform.position, adjacencyRadius, Color.yellow);
        }
    }

    private IEnumerator TickLoop()
    {
        var wait = new WaitForSeconds(tickSeconds);
        while (true)
        {
            ResourceManager.Instance.Add(gathererType, perSecond);
            float total = ResourceManager.Instance.Get(gathererType);
            yield return wait;
        }
    }

    private bool IsNextToCorrectResource(out Collider[] hitsOut)
    {
        hitsOut = Physics.OverlapSphere(transform.position, adjacencyRadius, resourceLayer);
        for (int i = 0; i < hitsOut.Length; i++)
        {
            var tile = hitsOut[i].GetComponent<ResourceTile>();
            if (tile != null && tile.type == gathererType)
                return true;
        }
        return false;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, adjacencyRadius);
    }
#endif

    // Simple runtime circle for debug (XZ plane)
    private static void DebugDrawCircle(Vector3 center, float radius, Color color, int segments = 32)
    {
        float step = Mathf.PI * 2f / segments;
        Vector3 prev = center + new Vector3(Mathf.Cos(0f) * radius, 0f, Mathf.Sin(0f) * radius);
        for (int i = 1; i <= segments; i++)
        {
            float a = step * i;
            Vector3 next = center + new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius);
            Debug.DrawLine(prev + Vector3.up * 0.05f, next + Vector3.up * 0.05f, color, 0f, false);
            prev = next;
        }
    }
}
