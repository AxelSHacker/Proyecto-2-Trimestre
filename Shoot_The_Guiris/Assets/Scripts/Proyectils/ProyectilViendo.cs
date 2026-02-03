using UnityEngine;

public class ProyectilViendo : Proyectil
{
    [Header("Variables")]
    [SerializeField] ParticleSystem _particulasViento;
    [SerializeField] float _fuerzadelViento;

    public override void Initialize()
    {
        //Forzamos el estado activo de la base
        base.Initialize();
        // 4. Lógica de partículas REFORZADA
        if (_particulasViento != null)
        {
            var _particleRenderer = _particulasViento.GetComponent<ParticleSystemRenderer>();
            if (_particleRenderer != null) _particleRenderer.enabled = true;
            
            _particulasViento.Clear();
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





