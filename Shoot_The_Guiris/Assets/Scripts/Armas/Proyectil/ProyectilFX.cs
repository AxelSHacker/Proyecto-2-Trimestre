using UnityEngine;
using UnityEngine.Events;

public class ProyectilFX : CustomMonoBehaviour
{
   public UnityEvent OnInitialize;
   public UnityEvent<Vector3> OnImpact;
   [SerializeField] Proyectil _proyectil;
   [SerializeField] MeleeWeapons _meeeWeapons;
   public override void EditorInit()
   {

   }
   void OnEnable()
   {
      if (_proyectil != null)
      {
         _proyectil.OnInitialize += OnInitialize.Invoke;
         _proyectil.OnImpact += OnImpact.Invoke;
      }
      else if (_meeeWeapons != null)
      {
         _meeeWeapons.OnInitialize += OnInitialize.Invoke;
         _meeeWeapons.OnImpact += OnImpact.Invoke;
      }

   }

   void OnDestroy()
   {
      if (_proyectil != null)
      {
         _proyectil.OnInitialize += OnInitialize.Invoke;
         _proyectil.OnImpact += OnImpact.Invoke;
      }

      else if (_meeeWeapons != null)
      {
         _meeeWeapons.OnInitialize += OnInitialize.Invoke;
         _meeeWeapons.OnImpact += OnImpact.Invoke;
      }
   }
}



