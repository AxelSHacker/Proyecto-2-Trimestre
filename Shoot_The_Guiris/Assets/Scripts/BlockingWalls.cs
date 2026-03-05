using System;
using UnityEngine;

public class BlockingWalls : MonoBehaviour
{
    [SerializeField] int _moneyToUnlock;
    [SerializeField] Vector3 _radio;
    [SerializeField] Vector3 _offset;
    [SerializeField] LayerMask _player;
    [SerializeField] PlayerControler _playerControler;
    bool _jugadorCerca = false;
    float _timer = 1f;
    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f)
        {
            CheckBox();
            _timer = 0;
        }

        if (_jugadorCerca && _playerControler.comprar)
        {
            IntentarComprar();
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
            Destroy(gameObject);
        }
    }
}
