using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoaderButton : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] private string sceneName; 
    [SerializeField] private string[] prerequisiteScenes; 
    [SerializeField] private bool requiresDeck = false; 

    [Header("UI References")]
    [SerializeField] private Image nodeImage;   // circle UI Image
    [SerializeField] private GameObject emptyDeckPopup; // assign a popup panel in the Inspector

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button == null)
        {
            Debug.LogWarning("No Button component found on this GameObject.");
            return;
        }

        button.onClick.AddListener(LoadScene);

        UpdateNodeVisual();
    }

    private void OnEnable()
    {
        UpdateNodeVisual();
    }

    private void UpdateNodeVisual()
    {
        if (string.IsNullOrEmpty(sceneName) || nodeImage == null) return;

        bool prerequisitesMet = Levels.HasCompletedPrerequisites(prerequisiteScenes);
        bool isComplete = Levels.IsLevelComplete(sceneName);

        button.interactable = prerequisitesMet;

        if (isComplete)
        {
            nodeImage.color = Color.green; // Completed
        }
        else if (!prerequisitesMet)
        {
            nodeImage.color = Color.grey;   // Locked
        }
        else
        {
            nodeImage.color = Color.red;  // Unlocked but not complete
        }
    }

    private void LoadScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name not set in Inspector.");
            return;
        }

        if (requiresDeck && IsDeckEmpty())
        {
            Debug.LogWarning("Cannot start level: Deck is empty!");
            if (emptyDeckPopup != null) emptyDeckPopup.SetActive(true);
            return;
        }

        SceneManager.LoadScene(sceneName);
        Levels.DebugLevelStatus();
    }

    private bool IsDeckEmpty()
    {
        const string PrefKey = "CardSlot";
        for (int i = 0; i < 8; i++) // check all slots
        {
            int savedIndex = PlayerPrefs.GetInt($"{PrefKey}{i}", 0);
            if (savedIndex > 0) return false; // found a card
        }
        return true;
    }
}
