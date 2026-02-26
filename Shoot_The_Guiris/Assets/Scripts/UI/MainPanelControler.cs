
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainPanelControler : MonoBehaviour
{
    [SerializeField] GameObject _settingsPanel;
    [SerializeField] GameObject _mainPanel;

    void Start()
    {
        _mainPanel.SetActive(true);
        _settingsPanel.SetActive(false);
    }


    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
