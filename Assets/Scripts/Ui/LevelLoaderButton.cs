using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoaderButton : MonoBehaviour
{
    [SerializeField] private string sceneName; 
    [SerializeField] private string[] prerequisiteScenes; 
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.interactable = Levels.HasCompletedPrerequisites(prerequisiteScenes);
            button.onClick.AddListener(LoadScene);
        }
        else
        {
            Debug.LogWarning("No Button component found on this GameObject.");
        }
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            Levels.DebugLevelStatus();
        }
        else
        {
            Debug.LogError("Scene name not set in Inspector.");
        }
    }
}
