using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Misil : PoolEntity
{
   [Header("Misil")]
   [SerializeField] float _damage;
   [SerializeField] float _damageRadio;
   [SerializeField] float _pathTime = 1f;
   float _pathTimer;

   [SerializeField] LayerMask _disparable;
   [SerializeField] Vector3 _startPosicion;
   [SerializeField] Vector3 _posicionTarget;
   Vector3 _posiciondedisparo;
   IDamageabe<float> _damageable;
   public UnityEvent OnInizialize;
   public UnityEvent OnImpact;
   public UnityEvent OnDeactivate;

   void Start()
   {

   }

   void Update()
   {
      transform.position = _posiciondedisparo + Vector3.Slerp(_startPosicion - _posiciondedisparo, _posicionTarget - _posiciondedisparo, 1 - _pathTimer / _pathTime);

      _pathTimer -= Time.deltaTime;
   }
   void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, _damageRadio);
   }
   void OnTriggerEnter(Collider other)
   {
      if (!IsActive) return;

      if ((_disparable & (1 << other.gameObject.layer)) != 0)
      {
         Collider[] impactos = Physics.OverlapSphere(transform.position, _damageRadio, _disparable);
         foreach (Collider impact in impactos)
         {
            _damageable = null;
            if (impact.TryGetComponent(out _damageable))
            {
               _damageable.TakeDamag(_damage, transform.position);

            }
            if (impact.TryGetComponent(out EnemigoIngles enemigo))
            {
               enemigo.Cegar();
            }
         }
         _posicionTarget = transform.position;
         OnImpact?.Invoke();
         ReturnToPool();
      }
   }



   public void IniciarMisil(Vector3 startPoint, Vector3 targetPoint, Vector3 shooterpoint)
   {
      _startPosicion = startPoint;
      _posicionTarget = targetPoint;
      _posiciondedisparo = shooterpoint;
   }
   [ContextMenu("Inizialize")]
   #region PoolEntity
   public override void Initialize()
   {
      base.Initialize();
      _pathTimer = _pathTime;
      OnInizialize?.Invoke();
   }
   public override void Deactivate()
   {
      base.Deactivate();
      OnDeactivate?.Invoke();
   }

   #endregion
}







