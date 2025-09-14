using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    private enum VolumeType
    {
        Master,
        Music,
        SFX
    }
    [Header("Type")]
    [SerializeField] private VolumeType volumeType;

    private Slider volumeSlider;
    private void Awake()
    {
        volumeSlider = this.GetComponentInChildren<Slider>();
    
    float saved = volumeType switch
    {
        VolumeType.Master => PlayerPrefs.GetFloat("MasterVolume", AudioManager.instance.masterVolume),
        VolumeType.Music  => PlayerPrefs.GetFloat("MusicVolume", AudioManager.instance.musicVolume),
        VolumeType.SFX    => PlayerPrefs.GetFloat("SFXVolume", AudioManager.instance.sfxVolume),
        _ => volumeSlider.value
    };
    volumeSlider.value = saved;
}
    private void Update()
    {
        switch (volumeType)
        {
            case VolumeType.Master:
                volumeSlider.value = AudioManager.instance.masterVolume;
                break;
            case VolumeType.Music:
                volumeSlider.value = AudioManager.instance.musicVolume;
                break;
            case VolumeType.SFX:
                volumeSlider.value = AudioManager.instance.sfxVolume;
                break;
            default:
                Debug.LogWarning("VolumeType not set in VolumeSlider");
                break;
        }
    }
  public void OnSliderValueChanged()
{
    float val = volumeSlider.value;
    switch (volumeType)
    {
        case VolumeType.Master:
            AudioManager.instance.masterVolume = val;
            PlayerPrefs.SetFloat("MasterVolume", val);
            break;
        case VolumeType.Music:
            AudioManager.instance.musicVolume = val;
            PlayerPrefs.SetFloat("MusicVolume", val);
            break;
        case VolumeType.SFX:
            AudioManager.instance.sfxVolume = val;
            PlayerPrefs.SetFloat("SFXVolume", val);
            break;
        default:
            Debug.LogWarning("VolumeType not set in VolumeSlider");
            break;
    }

    PlayerPrefs.Save();
    }

}
