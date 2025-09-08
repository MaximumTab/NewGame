using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinishLevel : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(FinishLVL);
        }
        else
        {
            Debug.LogWarning("No Button component found on this GameObject.");
        }
    }

    private void FinishLVL()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        Levels.MarkLevelComplete(currentScene);
        Debug.Log("Level finished: " + currentScene);

        // Show debug of all levels
        Levels.DebugLevelStatus();

        SceneManager.LoadScene(1); // back to Level Select
    }

    public void FailedLVL()
    {
        SceneManager.LoadScene(1);
    }
}
