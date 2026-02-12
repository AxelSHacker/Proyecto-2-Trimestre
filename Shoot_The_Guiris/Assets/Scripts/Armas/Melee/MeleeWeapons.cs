using System;
using UnityEngine;

public class MeleeWeapons : CustomMonoBehaviour
{
   public Action OnInitialize;
   public Action<Vector3> OnImpact;
   [SerializeField] float _fuerzaEscudo;
   [SerializeField] ParticleSystem _trailParticles;
   public float _damage;
   public LayerMask _shootableLayers;

   public override void EditorInit()
   {

   }
   void OnTriggerEnter(Collider other)
   {
      if ((_shootableLayers & (1 << other.gameObject.layer)) != 0 && other.TryGetComponent(out IDamageabe<float> damageable))
      {
         damageable.TakeDamag(_damage, transform.position);

         if (other.TryGetComponent(out CharacterController component))
         {
            
            Vector3 direccion = (other.transform.position - transform.position).normalized;
            direccion.y = 0.1f;
            component.Move(direccion * _fuerzaEscudo);
         }
         OnImpact?.Invoke(transform.position);
      }
   }
}






      






