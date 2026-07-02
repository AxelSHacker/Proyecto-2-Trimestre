
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    [SerializeField] WaveController _waveController;
    [SerializeField] GameObject[] _doors;
    [SerializeField] Transform _spawnPositionEnemy;
    [SerializeField] GameObject[] _tutorialPanels;
    [SerializeField] Transform _tutorialSpawnPosition;
    [SerializeField] GameObject _player;
    int _actualPanel = 0;
    void Start()
    {
        if (!_waveController.tutorial) { gameObject.SetActive(false); return; }
        else { _player.transform.position = _tutorialSpawnPosition.position; }
    }

    void Update()
    {
        CheckTutorial();
    }
    public void OnSkipText(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            TutorialPanels();
        }
    }

    private void CheckTutorial()
    {
        if (_waveController.tutorial)
        {
            for (int i = 0; i < _doors.Length; i++)
            {
                if (_doors[i] == null)
                {
                    DataManager.Instance.tutorial = false;
                    gameObject.SetActive(false);
                }
            }
        }
    }
    public void TutorialPanels()
    {
        _actualPanel++;

        for (int i = 0; i < _tutorialPanels.Length; i++)
        {
            if (i >= _tutorialPanels.Length) gameObject.SetActive(false);
            if (i == _actualPanel)
            {
                _tutorialPanels[i].SetActive(true);
            }
            else { _tutorialPanels[i].SetActive(false); }
        }
        if (_actualPanel == 2)
        {
            PoolManager.Instance.Pull("Enemiga Inglesa 1H", _spawnPositionEnemy.position, Quaternion.identity);

        }
        else if (_actualPanel == 3)
        {
            PoolManager.Instance.Pull("Enemigo 2H", _spawnPositionEnemy.position, Quaternion.identity);
            PoolManager.Instance.Pull("Enemiga Pistola", _spawnPositionEnemy.position, Quaternion.identity);
        }
    }

}
