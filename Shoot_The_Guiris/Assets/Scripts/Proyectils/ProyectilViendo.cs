using UnityEngine;

public class ProyectilViendo : Proyectil
{
    [Header("Variables")]
    [SerializeField] ParticleSystem _particulasViento;
    [SerializeField] float _fuerzadelViento;

    public override void Initialize()
    {

        //Ajustamos los componentes que viene en el codigo heredado y el poolEntity
        if (_rB != null) _rB.isKinematic = true;
        if (_collider != null) _collider.enabled = false;


        //Disparamos las particulas
        if (_particulasViento != null)
        {
            _particulasViento.gameObject.SetActive(true);
            _particulasViento.Clear();
            _particulasViento.Play();
        }

        float lifeTime = 5f;
        Invoke("ReturnToPool", lifeTime);
    }

    public override void Deactivate()
    {
        if (_particulasViento != null)
        {
            _particulasViento.Stop();
        }

        CancelInvoke("ReturnToPool");
    }

    //Modulo de collision propio de las particulas

    private void OnParticleCollision(GameObject other)
    {
        Rigidbody rigidbody = other.GetComponent<Rigidbody>();

        if ((_shootableLayers & (1 << other.gameObject.layer)) != 0)
        {
            Vector3 direccion = transform.forward;
            //Pequenio levantamiento para evitar la friccion
            direccion.y = 0.1f;
            rigidbody.AddForce(direccion * _fuerzadelViento, ForceMode.Impulse);

        }
        Debug.Log("Soy Concha, entro");
    }

}





