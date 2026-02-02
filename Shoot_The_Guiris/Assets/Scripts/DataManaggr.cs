using UnityEngine;

public class DataManager : MonoBehaviour
{
    //Puntuacion maxima registrada
    public int maxScore = 0;

    public int tutorialScore = 0;

    public int actualGameScore = 0;

    public float musicVolumen = 0f;

    public float sfxVolumen = 0f;
    private static DataManager _instance;

    public static DataManager Instance => _instance;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Load();
            LoadVolumenParameters();
        }
        else
        {
            Destroy(this);
        }

    }
    /// <summary>
    /// guardado de datos n los playeperfs
    public void Save()
    {
        PlayerPrefs.SetInt("maxScore", maxScore);
        PlayerPrefs.SetInt("tutorialMaxScore", tutorialScore);
        PlayerPrefs.SetInt("GameScore", actualGameScore);
        
    }
    public void SaveVolumenParameters()
    {
        PlayerPrefs.SetFloat("MusicVolumen", musicVolumen);
        PlayerPrefs.SetFloat("SFXVolumen", sfxVolumen);
    }
    
    /// <summary>
    /// Carga de datos desde los playerprefs.
    /// </summary>
    public void Load()
    {
        if (!PlayerPrefs.HasKey("maxScore")) return;
        maxScore = PlayerPrefs.GetInt("maxScore");
        tutorialScore = PlayerPrefs.GetInt("tutorialMaxScore");
        actualGameScore = PlayerPrefs.GetInt("GameScore");
        
    }

    public void LoadVolumenParameters()
    {
        musicVolumen = PlayerPrefs.GetFloat("MusicVolumen");
        sfxVolumen = PlayerPrefs.GetFloat("SFXVolumen");
    }
    /// <summary>
    /// Lmpia toda la informacion guardada en los player
    /// </summary>
    public void CleaData()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ClearMaxScore()
    {
        PlayerPrefs.DeleteKey("maxScore");
    }


}
