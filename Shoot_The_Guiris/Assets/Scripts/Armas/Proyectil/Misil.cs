
using UnityEngine;
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
   IDamageabe<float> iDamageable;

   Vector3 _posiciondedisparo;
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
            if (TryGetComponentInParent(impact))
            {
               iDamageable.TakeDamag(_damage, transform.position);
               EnemigoIngles enemigo = other.GetComponentInParent<EnemigoIngles>();
               if (enemigo != null)
               {
                  enemigo.Cegar();
               }
            }
         }
         _posicionTarget = transform.position;
         OnImpact?.Invoke();
         ReturnToPool();
      }
   }

   #region Funciones
   private bool TryGetComponentInParent(Collider collider)
   {
      iDamageable = collider.GetComponentInParent<IDamageabe<float>>();
      return iDamageable != null;
   }

   #endregion
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















