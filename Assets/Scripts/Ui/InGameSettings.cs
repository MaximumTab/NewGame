using UnityEngine;
using UnityEngine.SceneManagement;

public class InGameSettings : MonoBehaviour
{
    
    public GameObject settingsMenuPanel;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void settingsBtn()
    {
        Time.timeScale = 0f;
        settingsMenuPanel.SetActive(true);
        
    }

    public void closeBtn()
    {
        
        Time.timeScale = 1f;
        settingsMenuPanel.SetActive(false);
    }

    public void leaveLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }
}
