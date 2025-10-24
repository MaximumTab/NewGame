using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelContainer : MonoBehaviour
{
    [Header("Level Info")]
    [SerializeField] private string sceneName; 
    [SerializeField] private string[] prerequisiteScenes; 
    [SerializeField] private bool requiresDeck = false; 

    [Header("References")]
    [SerializeField] private Material mat;   // Object's material
    [SerializeField] private GameObject emptyDeckPopup; // assign a popup panel in the Inspector
    [SerializeField] private TMP_Text LevelName;

    private Button button;

    private void Awake()
    {
        UpdateNodeVisual();
        mat = GetComponent<MeshRenderer>().material;
    }

    private void OnEnable()
    {
        UpdateNodeVisual();
    }

    void OnMouseDown()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()||!Levels.HasCompletedPrerequisites(prerequisiteScenes)) return;
        LoadScene();
    }

    private void UpdateNodeVisual()
    {
        if (string.IsNullOrEmpty(sceneName) || mat == null) return;

        bool prerequisitesMet = Levels.HasCompletedPrerequisites(prerequisiteScenes);
        bool isComplete = Levels.IsLevelComplete(sceneName);
        
        LevelName.text = sceneName;

        if (isComplete)
        {
            mat.color = Color.green; // Completed
        }
        else if (!prerequisitesMet)
        {
            mat.color = Color.grey;   // Locked
        }
        else
        {
            mat.color = Color.red;  // Unlocked but not complete
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
