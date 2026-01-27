using System;
using UnityEngine;

public class PoolEntity : CustomMonoBehaviour
{
   public static Action<PoolEntity> OnReturnToPool;
   [SerializeField] string _poolID;
   [SerializeField] Renderer[] _renderers;
   [SerializeField] bool _isActive;
   public override void EditorInit()
   {
      _renderers = GetComponentsInChildren<Renderer>();
   }

   public virtual void Initialize()
   {
      EnableRenderers(true);
      _isActive = true;
   }

   public virtual void Deactivate()
   {
      EnableRenderers(false);
      _isActive = false;
   }

   public void ReturnToPool()
   {
      Deactivate();
      OnReturnToPool?.Invoke(this);
   }

   private void EnableRenderers(bool enable)
   {
      foreach (Renderer ren in _renderers)
      {
         ren.enabled = enable;
      }
   }
}



