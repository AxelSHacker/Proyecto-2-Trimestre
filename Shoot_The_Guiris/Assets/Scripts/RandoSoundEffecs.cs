
using UnityEngine;

public class RandoSoundEffecs : MonoBehaviour
{
    [SerializeField] AudioClip[] _effectClips;
    [SerializeField] AudioSource _audioSource;
    float minPitch = 0.85f;
    float maxPith = 1.15f;

    float _soundChance = 50f;
    public void PlayRandomSoundEffect()
    {
        
        if (_effectClips.Length == 0 || _audioSource == null)
        {
            Debug.LogWarning("No audio clips or audio source assigned.");
            return;
        }

        if (Random.Range(0f, 100f) <= _soundChance)
        {
            
            int randomIndex = Random.Range(0, _effectClips.Length);
            float randomPitch = Random.Range(minPitch, maxPith);
            AudioClip randomClip = _effectClips[randomIndex];
            _audioSource.pitch = randomPitch;
            _audioSource.PlayOneShot(randomClip);

        }
    }
}


