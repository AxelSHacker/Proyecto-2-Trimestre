using Unity.VisualScripting;
using UnityEngine;

public class RandoSoundEffecs : MonoBehaviour
{
    [SerializeField] AudioClip[] _effectClips;
    [SerializeField] AudioSource _audioSource;
    
    public void PlayRandomSoundEffect()
    {
        if (_effectClips.Length == 0 || _audioSource == null)
        {
            Debug.LogWarning("No audio clips or audio source assigned.");
            return;
        }

        int randomIndex = Random.Range(0, _effectClips.Length);
        AudioClip randomClip = _effectClips[randomIndex];
        _audioSource.PlayOneShot(randomClip);
    }
}
