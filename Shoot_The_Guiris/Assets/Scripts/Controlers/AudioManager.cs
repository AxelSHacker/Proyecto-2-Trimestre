using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    private static AudioManager _instance;
    public static AudioManager Instance => _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public float GetMasterVolume()
    {
        bool result = _audioMixer.GetFloat("MasterVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        if (!result)
        {
            Debug.LogWarning("Could not get MasterVolume from AudioMixer");
        }
            
        return volume;
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public float GetMusicVolume()
    {
        bool result = _audioMixer.GetFloat("MusicVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        if (!result)
        {
            Debug.LogWarning("Could not get MusicVolume from AudioMixer");
        }
            
        return volume;
    }
    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public float GetEffectsVolume()
    {
        bool result = _audioMixer.GetFloat("EffectsVolume", out float volume);
        volume = Mathf.Pow(10, volume / 20);
        if (!result)
        {
            Debug.LogWarning("Could not get EffectsVolume from AudioMixer");
        }
            
        return volume;
    }
    public void SetEffectsVolume(float volume)
    {
        _audioMixer.SetFloat("EffectsVolume", Mathf.Log10(volume) * 20);
    }
}
