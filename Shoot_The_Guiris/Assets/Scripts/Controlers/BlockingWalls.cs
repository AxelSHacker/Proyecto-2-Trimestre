using System;
using TMPro;
using UnityEngine;

public class BlockingWalls : MonoBehaviour
{
    public static System.Action<GameObject> OnWallDestroy;
    public static Action OnWallCounter;
    [SerializeField] int _moneyToUnlock;

    [SerializeField] PlayerControler _playerControler;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TextMeshProUGUI _moneyText;
    void OnDisable()
    {
        if (_canvasGroup != null)
            _canvasGroup.alpha = 0;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerControler playerControler))
        {
            if (_playerControler == null) _playerControler = playerControler;

            _canvasGroup.alpha = 1;
            _moneyText.text = _moneyToUnlock.ToString() + " € Button[B]";
        }
    }
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out PlayerControler playerControler))
        {
            if (playerControler.comprar) { IntentarComprar(); Debug.Log("Comprando"); }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out PlayerControler playerControler))
        {
            _canvasGroup.alpha = 0;
        }
    }
    private void IntentarComprar()
    {
        if (_playerControler.Money >= _moneyToUnlock)
        {
            Debug.Log("Comprado");
            _playerControler.QuitarDinero(_moneyToUnlock);
            OnWallDestroy?.Invoke(gameObject);
            OnWallCounter?.Invoke();
            Destroy(gameObject);
        }
    }
}
