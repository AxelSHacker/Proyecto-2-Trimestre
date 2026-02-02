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

   [Header("Proyectil")]
   [SerializeField] float _damage;
   [SerializeField] float _speed;
   public float _lifetime;
   float _lifeTimerTmp;
   public LayerMask _shootableLayers;
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
         OnImpact?.Invoke(transform.position);
         ReturnToPool();
      }

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
      _trailParticles.Stop();
   }


   #endregion
}



