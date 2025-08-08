using UnityEngine;
using TMPro;


public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;
     

     public void SetGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
