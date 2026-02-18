using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Misil : PoolEntity
{
   [Header("Misil")]
   [SerializeField] float _damage;
   [SerializeField] float _damageRadio;
   [SerializeField] float _velocidad;
   [SerializeField] float _vidadelMisil;
   float _tiempodeMisil;
   [SerializeField] LayerMask _disparable;
   [SerializeField] Vector3 _startPosicion;
   [SerializeField] Vector3 _posicionTarget;
   [SerializeField] Vector3 _posiciondedisparo;
   IDamageabe<float> _damageable;
   public UnityEvent OnInizialize;
   public UnityEvent OnImpact;
   public UnityEvent OnDeactivate;

   void Start()
   {

   }

   void Update()
   {
      if (_tiempodeMisil < -1 && IsActive) ReturnToPool();

      transform.position = Vector3.Slerp(_startPosicion - _posiciondedisparo, _posicionTarget - _posiciondedisparo, 1 - _tiempodeMisil / _vidadelMisil);

      _tiempodeMisil -= Time.deltaTime;
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
            if (other.TryGetComponent(out _damageable))
            {
               _damageable.TakeDamag(_damage, transform.position);
            }
         }

         _posicionTarget = transform.position;
         OnImpact?.Invoke();
         ReturnToPool();
      }
   }
    #region PoolEntity
    public override void Initialize()
    {
        base.Initialize();
        _tiempodeMisil = _vidadelMisil;
    }
    public override void Deactivate()
    {
        base.Deactivate();
        OnDeactivate?.Invoke();
    }

   #endregion
}



