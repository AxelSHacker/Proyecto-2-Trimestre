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
            _moneyText.text = _moneyToUnlock.ToString() + " € Button[B]";

            if (_playerControler.comprar) IntentarComprar();
        }
        else
        {
            _textTimer += Time.deltaTime;
            if (_textTimer >= 1.5f)
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
        //Solo vamos a comprobar si es mayor que 0, asi que no necesitamos mas capacida de buffer
        Collider[] colliderBuffer = new Collider[1];
        //Comprobamos si hay contacto con el suelo, lo hacemos mediant un OvrlapboxnonAlloc,
        //para no consumir mas memoria de la necesaria ya que esta funcion la vamos a hacer de manera continua
        Physics.OverlapBoxNonAlloc(transform.position,
                                    _radio / 2f,
                                    colliderBuffer,
                                    transform.rotation,
                                    _player);
        //Actualitzamos el estado de _grounded
        _jugadorCerca = colliderBuffer[0] != null;
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
