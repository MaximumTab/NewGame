using TMPro;
using UnityEngine;

public class CardTooltip : MonoBehaviour
{
    public static CardTooltip Instance { get; private set; }

    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descText;

    private Canvas rootCanvas;
    private CanvasGroup cg;

    private void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        cg = panel.GetComponent<CanvasGroup>();
        if (!cg) cg = panel.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;

        rootCanvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        if (cg.alpha > 0)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootCanvas.transform as RectTransform,
                Input.mousePosition,
                rootCanvas.worldCamera,
                out pos
            );
            panel.anchoredPosition = pos + new Vector2(16, -16);
        }
    }

    public void Show(CardTooltipData data)
    {
        Debug.Log("[Tooltip] UnlockText object: " + unlockText.name);
        nameText.text = data.DisplayName;
        descText.text = data.Description;
        costText.text = data.CostInfo;

        // Unlock text
        bool hasUnlock = !string.IsNullOrEmpty(data.UnlockInfo);
        unlockText.gameObject.SetActive(hasUnlock);
        if (hasUnlock)
        {
            unlockText.text = $"<color=#FF0000>{data.UnlockInfo}</color>";
            unlockText.fontStyle = FontStyles.Italic;
        }
        // Locked styling: fade other sections
        if (data.IsLocked)
        {
            Color faded = new Color(1f, 1f, 1f, 0.5f);
            descText.color = faded;
            costText.color = faded;
            nameText.color = Color.white; // or faded too if you want
        }
        else
        {
            descText.color = Color.white;
            costText.color = Color.white;
            nameText.color = Color.white;
        }

        cg.alpha = 1;
    }

    public void Hide()
    {
        cg.alpha = 0;

        nameText.text = "";
        descText.text = "";
        costText.text = "";
        unlockText.text = "";
        unlockText.gameObject.SetActive(false);
    }
}
