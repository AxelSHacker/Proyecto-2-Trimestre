using UnityEngine;

public class SchieldScript : MeleeWeapons
{
   [SerializeField] float _fuerzaEscudo;
   public override void EditorInit()
   {

   }

   void OnTriggerEnter(Collider other)
   {
      if (other.TryGetComponent(out Rigidbody _rB))
      {
         if ((_shootableLayers & (1 << other.gameObject.layer)) != 0 && other.TryGetComponent(out IDamageabe<float> damageable))
         {

            damageable.TakeDamag(_damage, transform.position);
            Vector3 direccion = transform.forward;
            direccion.y = 0.1f;
            _rB.AddForce(direccion * _fuerzaEscudo, ForceMode.Impulse);


         }
         OnImpact?.Invoke(transform.position);
      }

   }
}



