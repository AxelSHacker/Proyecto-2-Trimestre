using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource _musicSource;
    [SerializeField] List<AudioClip> _musicClips;
    List<AudioClip> _listaActual = new List<AudioClip>();
    private static MusicManager instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);
    }
        
        

    void Start()
    {
        GenerarciondeListadeReproduccion();
        ReproducirMusica();
    }



    // Update is called once per frame
    void Update()
    {
        if (!_musicSource.isPlaying)
        {
            ReproducirMusica();
        }
    }

    private void GenerarciondeListadeReproduccion()
    {
        _listaActual = new List<AudioClip>(_musicClips);

        for (int i = 0; i < _musicClips.Count; i++)
        {
            AudioClip temp = _listaActual[i];

            int randomIndex = Random.Range(i, _musicClips.Count);
            _listaActual[i] = _listaActual[randomIndex];
            _listaActual[randomIndex] = temp;

        }
    }

    private void ReproducirMusica()
    {
        if (_listaActual.Count == 0)
        {
            GenerarciondeListadeReproduccion();
        }
        AudioClip proximaCancion = _listaActual[0];
        _listaActual.RemoveAt(0);
        _musicSource.clip = proximaCancion;
        _musicSource.Play();
    }
}
