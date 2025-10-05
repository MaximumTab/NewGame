using TMPro;
using UnityEngine;

public class CardTooltip : MonoBehaviour
{
    public static CardTooltip Instance { get; private set; }

    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text costText;

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

    public void Show(string name, string desc, string cost)
    {
        nameText.text = name;
        descText.text = desc;
        costText.text = cost;
        cg.alpha = 1;
    }

    public void Hide()
    {
        cg.alpha = 0;
    }
}
