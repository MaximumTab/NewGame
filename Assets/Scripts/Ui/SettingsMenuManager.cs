using UnityEngine;
using TMPro;


public class SettingsMenuManager : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;

    void Start()
    {
        graphicsDropdown.ClearOptions();
        var options = new System.Collections.Generic.List<string>(QualitySettings.names);
        graphicsDropdown.AddOptions(options);

        // Load saved quality level, or use current if not set
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", QualitySettings.GetQualityLevel());
        graphicsDropdown.value = savedQuality;
        graphicsDropdown.RefreshShownValue();
        QualitySettings.SetQualityLevel(savedQuality);

        graphicsDropdown.onValueChanged.AddListener(delegate { SetGraphicsQuality(); });
    }

    public void SetGraphicsQuality()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
        PlayerPrefs.SetInt("GraphicsQuality", graphicsDropdown.value);
        PlayerPrefs.Save();
    }
}