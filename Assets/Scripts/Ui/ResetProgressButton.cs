using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ResetProgressButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ResetProgress);
        }
        else
        {
            Debug.LogWarning("No Button component found on this GameObject.");
        }
    }

    private void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("=== ALL LEVEL PROGRESS RESET ===");

        // Show the reset state for debugging
        Levels.DebugLevelStatus();
        SceneManager.LoadScene(0);
    }
}
