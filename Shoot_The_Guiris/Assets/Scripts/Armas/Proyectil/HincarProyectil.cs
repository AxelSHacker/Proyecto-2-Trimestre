using UnityEngine;

public class HincarProyectil : MonoBehaviour
{
    [SerializeField] Proyectil _proyectil;

    [SerializeField] float _hincarDuration = 5f;

    public void Hincar(Vector3 posicion)
    {
        _proyectil.esHincarProyectil = true;
        _proyectil._rB.linearVelocity = Vector3.zero;
        _proyectil._rB.isKinematic = true;
        _proyectil._collider.enabled = false;

        transform.position = posicion + (transform.forward * 0.5f);

        _proyectil._lifeTimerTmp = Time.time + _hincarDuration;
    }
}
