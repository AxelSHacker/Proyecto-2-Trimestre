using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _target; // Arrastra aquí a tu "Samuel_Controller"

    [Header("Configuración")]
    [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, 0f); // Posición relativa
    [SerializeField] private Quaternion _rotationOffset = new Quaternion(0f, 0f, 0f, 0f); // Rotación relativa
    [SerializeField] private float _smoothSpeed = 5f; // Suavizado (Lerp)

    void LateUpdate()
    {
        if (_target == null) return;

        // 1. Calculamos la posición deseada
        Vector3 desiredPosition = _target.position + _offset;
        Quaternion desiredRotation = _target.rotation * _rotationOffset;

        // 2. Suavizamos el movimiento
        // Lerp interpola entre la posición actual y la deseada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);

        // 3. Aplicamos la posición
        transform.position = smoothedPosition;

        // 4. (Opcional) Hacer que la cámara siempre mire al jugador
        // transform.LookAt(_target); 
    }
}

