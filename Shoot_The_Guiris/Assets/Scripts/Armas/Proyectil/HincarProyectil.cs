using UnityEngine;

public class HincarProyectil : MonoBehaviour
{
    [SerializeField] Proyectil _proyectil;

    [SerializeField] float _hincarDuration = 5f;

    public void Hincar(Vector3 posicion, Transform impactado)
    {
        _proyectil.esHincarProyectil = true;
        
        //Desactivar Fisicas
        _proyectil._rB.linearVelocity = Vector3.zero;
        _proyectil._rB.isKinematic = true;
        _proyectil._collider.enabled = false;

        //Colocamos en el punto exacto de impacto
        transform.position = posicion + (transform.forward * 0.5f);

        //Emparentar asegurando el mantenga su posicion en el mundo
        gameObject.transform.SetParent(impactado);

        _proyectil._lifeTimerTmp = Time.time + _hincarDuration;
    }
}
