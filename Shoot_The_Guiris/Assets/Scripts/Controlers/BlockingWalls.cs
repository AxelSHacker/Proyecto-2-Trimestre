using System;
using TMPro;
using UnityEngine;

public class BlockingWalls : MonoBehaviour
{
    public static System.Action<GameObject> OnWallDestroy;
    public static Action OnWallCounter;
    [SerializeField] int _moneyToUnlock;
    [SerializeField] Vector3 _radio;
    [SerializeField] Vector3 _offset;
    [SerializeField] LayerMask _player;
    [SerializeField] PlayerControler _playerControler;
    [SerializeField] CanvasGroup _canvasGroup;
    [SerializeField] TextMeshProUGUI _moneyText;
    bool _jugadorCerca = false;
    float _timer = 1f;
    float _textTimer = 2f;
    void Start()
    {
        
    }
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f)
        {
            CheckBox();
            _timer = 0;
        }

        if (_jugadorCerca)
        {
            _canvasGroup.alpha = 1;
            _moneyText.text = _moneyToUnlock.ToString() + " €";

            if (_playerControler.comprar) IntentarComprar();
        }
        else
        {
            _textTimer += Time.deltaTime;
            if (_textTimer >= 2f)
            {
                _canvasGroup.alpha = 0;
                _textTimer = 0;
            }
        }
        
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(_offset), transform.rotation, _radio);
        Gizmos.matrix = rotationMatrix;
        
        Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    private void CheckBox()
    {
        _jugadorCerca = Physics.CheckBox(transform.position, _radio, transform.rotation, _player);

    }
    private void IntentarComprar()
    {
        if (_playerControler.Money >= _moneyToUnlock)
        {
            _playerControler.QuitarDinero(_moneyToUnlock);
            OnWallDestroy?.Invoke(gameObject);
            OnWallCounter?.Invoke();
            Destroy(gameObject);
        }
    }
}
