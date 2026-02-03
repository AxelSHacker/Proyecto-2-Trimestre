using UnityEngine;
using UnityEngine.UI;

public class SettingPanelControlr : MonoBehaviour
{
    [SerializeField] Slider _masterVolumeSlider;
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _effectsVolumeSlider;

    void Start()
    {
        _masterVolumeSlider.value = AudioManager.Instance.GetMasterVolume();
        _musicVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        _effectsVolumeSlider.value = AudioManager.Instance.GetEffectsVolume();
    }

    public void UpdateMasterVollumen()
    {
        AudioManager.Instance.SetMasterVolume(_masterVolumeSlider.value);
    }
    public void UpdateMusicVollumen()
    {
        AudioManager.Instance.SetMusicVolume(_musicVolumeSlider.value);
    }
    public void UpdateEffectVollumen()
    {
        AudioManager.Instance.SetEffectsVolume(_effectsVolumeSlider.value);
    }
}  