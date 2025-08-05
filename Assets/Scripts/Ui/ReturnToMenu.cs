using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    public int sceneIndex = 0; 

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(sceneIndex);
    }
}
