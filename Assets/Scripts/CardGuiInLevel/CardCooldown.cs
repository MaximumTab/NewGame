using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCooldown : MonoBehaviour
{
    [SerializeField] Slider fill;          // radial or bar image to show remaining time (optional)
    [SerializeField] TMP_Text label;      // countdown text (optional)
    [SerializeField] CanvasGroup cg;      // to dim and block input (optional)
    [SerializeField] MonoBehaviour dragHandler; // your CardDrag script

    float readyAt = -1f;
    bool cooling;

    public bool IsCooling => cooling;

    public void Begin(float seconds)
    {
        if (seconds <= 0f) { End(); return; }
        readyAt = Time.unscaledTime + seconds;
        cooling = true;
        SetBlocked(true);
        UpdateVisuals();
        enabled = true;
    }

    public void BeginCooldown(float Seconds)
    {
        StartCoroutine(CoolDownCard(Seconds));
    }

    public IEnumerator CoolDownCard(float Seconds)
    {
        label.gameObject.SetActive(true);
        cooling = true;
        for (float i = Seconds; i >= 0; i -= Time.deltaTime)
        {
            fill.value = i / Seconds;
            label.text = "" + Mathf.Round(i*10)/10;
            yield return null;
        }

        cooling = false;
        label.text = "";
        label.gameObject.SetActive(false);
    }

    // void Update()
    // {
    //     if (!cooling) { enabled = false; return; }
    //     float remaining = readyAt - Time.unscaledTime;
    //     if (remaining <= 0f) { End(); return; }
    //     UpdateVisuals();
    // }

    void End()
    {
        cooling = false;
        SetBlocked(false);
        //if (fill) fill.fillAmount = 0f;
        if (label) label.text = "";
        enabled = false;
    }

    void SetBlocked(bool blocked)
    {
        if (dragHandler) dragHandler.enabled = !blocked;
        if (cg)
        {
            cg.interactable = !blocked;
            cg.blocksRaycasts = !blocked;
            cg.alpha = blocked ? 0.6f : 1f;
        }
    }

    void UpdateVisuals()
    {
        float remaining = Mathf.Max(0f, readyAt - Time.unscaledTime);
        if (label) label.text = Mathf.CeilToInt(remaining).ToString();
        if (fill)
        {
            // 1 at start, 0 when ready
            float total = Mathf.Max(remaining, 0.0001f) + (Time.unscaledTime - (readyAt - remaining));
            //fill.fillAmount = Mathf.InverseLerp(0f, total, remaining);
        }
    }
}
