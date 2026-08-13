using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);
    }
    public void LoadTutorial(string name)
    {
        DataManager.Instance.tutorial = true;
        SceneManager.LoadScene(name);
    }
}
