using System;
using UnityEngine;

public class MeleeWeapons : CustomMonoBehaviour
{
   public Action OnInitialize;
   public Action<Vector3> OnImpact;

   [SerializeField] ParticleSystem _trailParticles;
   [SerializeField] float _damage;
   public LayerMask _shootableLayers;

   public override void EditorInit()
   {

   }
   void OnTriggerEnter(Collider other)
   {
      Debug.Log("Soy concha, Entro");
      if ((_shootableLayers & (1 << other.gameObject.layer)) != 0)
      {
         if (other.TryGetComponent(out IDamageabe<float> damageable))
         {
            damageable.TakeDamag(_damage, transform.position);
         }
         OnImpact?.Invoke(transform.position);
         
      }
   }
}
      





