
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class TutorialController : MonoBehaviour
{
    [SerializeField] WaveController _waveController;
    [SerializeField] GameObject[] _doors;
    [SerializeField] Transform _spawnPositionEnemy;
    [SerializeField] GameObject[] _tutorialPanels;
    [SerializeField] CanvasGroup _tutorialCanvas;
    [SerializeField] Transform _tutorialSpawnPosition;
    [SerializeField] GameObject _player;
    int _actualPanel = 0;

    PoolEntity _enemigoTutorial;
    bool desactivar = false;

    void Start()
    {
        if (!DataManager.Instance.tutorial) { gameObject.SetActive(false); return; }
        else if (_player.TryGetComponent(out CharacterController controller))
        {
            controller.enabled = false;
            _player.transform.position = _tutorialSpawnPosition.position;
            controller.enabled = true;
        }
    }
    void Update()
    {
        CheckTutorial();

        if (_enemigoTutorial != null && _enemigoTutorial.gameObject.TryGetComponent(out EnemigoIngles component))
        {
            if (component.Currentealt == 0 && !desactivar)
            {
                _tutorialCanvas.alpha = 1f;
                desactivar = true;
            }
            else if (component.Currentealt > 0)
            {
                _tutorialCanvas.alpha = 0f;
            }
        }
    }
    public void OnSkipText(InputAction.CallbackContext context)
    {
        if (_tutorialCanvas.alpha == 0f) return;

        if (context.started)
        {
            TutorialPanels();
        }
    }
    private void CheckTutorial()
    {
        if (DataManager.Instance.tutorial)
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

        if (_actualPanel == 3)
        {
            _enemigoTutorial = PoolManager.Instance.Pull("Enemigo 2H", _spawnPositionEnemy.position, Quaternion.identity);
        }
        else if (_actualPanel > _tutorialPanels.Length)
        {
            _tutorialCanvas.alpha = 0f;
        }
    }
}
