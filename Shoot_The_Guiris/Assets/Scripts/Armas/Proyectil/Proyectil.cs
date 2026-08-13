using System;
using UnityEngine;

public class Proyectil : PoolEntity
{
   public Action OnInitialize;
   public Action<Vector3> OnImpact;

   [Header("Componentes")]
   public Rigidbody _rB;
   public Collider _collider;
   [SerializeField] ParticleSystem _trailParticles;
   IDamageabe<float> iDamageable;

   [Header("Proyectil")]
   [SerializeField] float _damage;
   [SerializeField] float _speed;
   public float _lifetime;
   public float _lifeTimerTmp;
   public LayerMask _shootableLayers;
   public bool esHincarProyectil = false;
   public override void EditorInit()
   {
      base.EditorInit();
      _collider = GetComponent<Collider>();
      _rB = GetComponent<Rigidbody>();
   }
   #region Unity Methods

   void Update()
   {
      if (!IsActive) return;

      if (_lifeTimerTmp < Time.time)
      {
         ReturnToPool();
      }
   }

   void OnTriggerEnter(Collider other)
   {
      if (!IsActive) return;

      if ((_shootableLayers & (1 << other.gameObject.layer)) != 0)
      {
         if (gameObject.TryGetComponent(out HincarProyectil hincarProyectil))
         {
            Vector3 puntoImpacto = other.ClosestPoint(transform.position);
            hincarProyectil.Hincar(puntoImpacto, other.transform);
         }
         if (TryGetComponentInParent(other))
         {
            iDamageable.TakeDamag(_damage, transform.position);

            EnemigoIngles enemigo = other.GetComponentInParent<EnemigoIngles>();
            if (enemigo != null && PoolID == "ProyectilCaca")
            {
               enemigo.RaletizacionCoroutina();
            }
         }
         OnImpact?.Invoke(transform.position);
         if (esHincarProyectil) return;
         ReturnToPool();
      }
   }
   #endregion


   #region Funciones
   private bool TryGetComponentInParent(Collider collider)
   {
      iDamageable = collider.GetComponentInParent<IDamageabe<float>>();
      return iDamageable != null;
   }

   #endregion




   #region PoolEntity Methods
   public override void Initialize()
   {
      base.Initialize();
      _collider.enabled = true;
      _rB.isKinematic = false;
      _trailParticles.Play();
      _rB.linearVelocity = transform.forward * _speed;
      _lifeTimerTmp = Time.time + _lifetime;

      OnInitialize?.Invoke();
   }

   public override void Deactivate()
   {
      base.Deactivate();
      _collider.enabled = false;
      _rB.isKinematic = true;
      esHincarProyectil = false;
      _trailParticles.Stop();
   }


   #endregion
}



