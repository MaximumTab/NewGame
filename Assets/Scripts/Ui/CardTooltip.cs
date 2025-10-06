using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardTooltip : MonoBehaviour
{
    public static CardTooltip Instance { get; private set; }

    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text unlockText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text descText;

    // Stat fields
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private TMP_Text rangeText;

    // NEW: Tower sprite
    [SerializeField] private Image towerImage;

    private Canvas rootCanvas;
    private CanvasGroup cg;

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cg = panel.GetComponent<CanvasGroup>();
        if (!cg) cg = panel.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.blocksRaycasts = false;
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
        nameText.text = data.DisplayName;
        descText.text = data.Description;
        costText.text = data.CostInfo;

        // Set stat texts
        hpText.text = !string.IsNullOrEmpty(data.MaxHP) ? $"HP: {data.MaxHP}" : "";
        damageText.text = !string.IsNullOrEmpty(data.Damage) ? $"DMG: {data.Damage}" : "";
        rangeText.text = !string.IsNullOrEmpty(data.Range) ? $"RNG: {data.Range}" : "";

        // Show tower sprite if present
        if (towerImage)
        {
            bool hasSprite = data.TowerSprite != null;
            towerImage.gameObject.SetActive(hasSprite);
            if (hasSprite) towerImage.sprite = data.TowerSprite;
        }

        bool hasUnlock = !string.IsNullOrEmpty(data.UnlockInfo);
        unlockText.gameObject.SetActive(hasUnlock);
        if (hasUnlock)
        {
            unlockText.text = $"<color=#FF0000>{data.UnlockInfo}</color>";
            unlockText.fontStyle = FontStyles.Italic;
        }

        // Handle locked fade
        Color faded = new Color(1f, 1f, 1f, data.IsLocked ? 0.5f : 1f);
        descText.color = faded;
        costText.color = faded;
        hpText.color = faded;
        damageText.color = faded;
        rangeText.color = faded;
        if (towerImage) towerImage.color = faded;

        cg.alpha = 1;
    }

    public void Hide()
    {
        cg.alpha = 0;
        nameText.text = "";
        descText.text = "";
        costText.text = "";
        unlockText.text = "";
        hpText.text = "";
        damageText.text = "";
        rangeText.text = "";
        unlockText.gameObject.SetActive(false);

        if (towerImage)
        {
            towerImage.sprite = null;
            towerImage.gameObject.SetActive(false);
        }
    }
}
