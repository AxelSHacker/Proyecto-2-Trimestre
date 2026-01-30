using UnityEngine;

public class ProyectilViendo : Proyectil
{
    [Header("Variables")]
    [SerializeField] ParticleSystem _particulasViento;
    [SerializeField] float _fuerzadelViento;

    public override void Initialize()
    {
        //Forzamos el estado activo de la base
        _isActive = true;
        CancelInvoke("ReturnToPool");

        //El Invoke de retorno SIEMPRE fuera de los IFs
        float tiempoVida = (_lifetime > 0) ? _lifetime : 2f;
        Invoke("ReturnToPool", tiempoVida);

        // 3. Ajuste de componentes físicos
        if (_rB != null) _rB.isKinematic = true;
        if (_collider != null) _collider.enabled = false;

        // 4. Lógica de partículas REFORZADA
        if (_particulasViento != null)
        {
            _particulasViento.gameObject.SetActive(true);
            
            // Parada total y reset de simulación
            _particulasViento.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _particulasViento.Simulate(0, true, true);
            
            var main = _particulasViento.main;
            main.duration = tiempoVida; // Sincronizamos duración con vida del objeto

            _particulasViento.Play(true);
        }
    }

    public override void Deactivate()
    {
        _isActive = false;
        if (_particulasViento != null)
        {
            _particulasViento.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
        CancelInvoke("ReturnToPool");
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.TryGetComponent(out Rigidbody rb))
        {
            if ((_shootableLayers & (1 << other.gameObject.layer)) != 0)
            {
                Vector3 direccion = transform.forward;
                direccion.y = 0.1f;
                rb.AddForce(direccion * _fuerzadelViento, ForceMode.Impulse);
                
            }
        }
    }
}





