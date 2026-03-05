using System.Collections;
using UnityEngine;

public class WindReact : MonoBehaviour
{
    [SerializeField] float _returnSpeed;
    [SerializeField] float _tiempodeEspera;
    Rigidbody _rb;
    bool _isPushed = false;
    Vector3 _initialPosition;
    Vector3 _initialScale;
    Quaternion _initialRotation;
    void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
        _initialScale = transform.localScale;
    }
    public void Push(Vector3 direction, float force)
    {
        if (_isPushed) return;
        _isPushed = true;

        _rb = gameObject.AddComponent<Rigidbody>();

        _rb.AddForce(direction * force * 3, ForceMode.Impulse);
        _rb.AddTorque(Random.insideUnitSphere * force, ForceMode.Impulse);

        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        yield return new WaitForSeconds(_tiempodeEspera);

        if (_rb != null) Destroy(_rb);
        _rb = null;

        transform.localPosition = _initialPosition + (Vector3.down * 2f);
        transform.localRotation = _initialRotation;
        transform.localScale = Vector3.zero;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * _returnSpeed;

            transform.localPosition = Vector3.Lerp(transform.localPosition, _initialPosition, t);
            transform.localScale = Vector3.Lerp(Vector3.zero, _initialScale, t);

            yield return null;
        }

        transform.localPosition = _initialPosition;
        transform.localScale = _initialScale;
        _isPushed = false;
    }
}


