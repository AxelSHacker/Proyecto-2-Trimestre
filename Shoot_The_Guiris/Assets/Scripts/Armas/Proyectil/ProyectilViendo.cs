using UnityEngine;

public class ProyectilViendo : Proyectil
{
    [Header("Variables")]
    [SerializeField] ParticleSystem _particulasViento;
    [SerializeField] float _fuerzadelViento;

    public override void Initialize()
    {
        base.Initialize();
        //Lógica de partículas REFORZADA
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
        if ((_shootableLayers & (1 << other.gameObject.layer)) != 0)
        { 
            if (other.TryGetComponent(out EnemigoIngles enemigo))
            {
                Vector3 direccion = transform.forward;
                direccion.y = 0.6f;
                enemigo.ImpactoViento(direccion * _fuerzadelViento);
            }
            else if (other.TryGetComponent(out WindReact windReact))
            {
                Vector3 direccion = transform.forward;
                direccion.y = 0.6f;
                windReact.Push(direccion, _fuerzadelViento);
            }
        }
    }
}









               







