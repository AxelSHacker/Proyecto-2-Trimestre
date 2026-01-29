using UnityEngine;
using UnityEngine.Events;

public class ProyectilFX : CustomMonoBehaviour
{
   public UnityEvent OnInitialize;
   public UnityEvent<Vector3> OnImpact;
   [SerializeField] Proyectil _proyectil;
   public override void EditorInit()
   {

   }
   void OnEnable()
   {
      _proyectil.OnInitialize += OnInitialize.Invoke;
      _proyectil.OnImpact += OnImpact.Invoke;
   }

   void OnDestroy()
   {
      _proyectil.OnInitialize -= OnInitialize.Invoke;
      _proyectil.OnImpact -= OnImpact.Invoke;
   }

}



