using System;
using UnityEngine;

public class PoolEntity : CustomMonoBehaviour
{
   public static Action<PoolEntity> OnReturnToPool;
   [SerializeField] string _poolID;
   [SerializeField] Renderer[] _renderers;
   public bool _isActive;

   public bool IsActive => _isActive;

   public string PoolID
   {
      get => _poolID;
      set => _poolID = value; 
   }
   public override void EditorInit()
   {
      _renderers = GetComponentsInChildren<Renderer>();
   }

   public virtual void Initialize()
   {
      _isActive = true;
      gameObject.SetActive(true);
      EnableRenderers(true);
   }

   public virtual void Deactivate()
   {
      _isActive = false;
      EnableRenderers(false);
   }

   public void ReturnToPool()
   {
      Deactivate();
      OnReturnToPool?.Invoke(this);
   }

   private void EnableRenderers(bool enable)
   {
      if (_renderers == null) return;
      
      foreach (Renderer ren in _renderers)
      {
         ren.enabled = enable;
      }
   }
}



