using System;
using UnityEngine;

public class DissolveBehaviour : CustomMonoBehaviour
{
   [SerializeField] Renderer[] _renderers;
   MaterialPropertyBlock _dissolvePropertBlock;
   [SerializeField] float _dissolveTime = 1f;
   [SerializeField] float _dissolveMaxHeight = 2f;
   [SerializeField] float _dissolveMinHeight = -2f;
   float _timer;
   float _currentrHeight;
   bool _isRunning;

   Action _dissolveCallBack;
   public override void EditorInit()
   {
      _renderers = GetComponentsInChildren<Renderer>();
   }
   void Awake()
   {
      _dissolvePropertBlock = new MaterialPropertyBlock();
   }
   void Start()
   {

   }

   void Update()
   {
      if (!_isRunning || _timer <= 0) return;

      foreach (Renderer renderer in _renderers)
      {
         renderer.GetPropertyBlock(_dissolvePropertBlock);
      }
   
      float t = 1 - (_timer / _dissolveTime);
      _currentrHeight = Mathf.Lerp(_dissolveMaxHeight, _dissolveMinHeight, t);
      _dissolvePropertBlock.SetFloat("CutoffHeight", _currentrHeight);
      foreach (Renderer renderer in _renderers)
      {
         renderer.SetPropertyBlock(_dissolvePropertBlock);
      }
         
      _timer -= Time.deltaTime;

      if (_timer <= 0)
      {
         StopDissolve();
      }
   }
      
      

   public void StartDissolve(Action callBack = null)
   {
      Debug.Log("entro");
      _isRunning = true;
      _timer = _dissolveTime;
      _dissolveCallBack = callBack;
   }

   public void StopDissolve()
   {
      _isRunning = false;
      _dissolveCallBack?.Invoke();
   }
   public void ResetDissolve()
   {
      foreach (Renderer renderer in _renderers)
      {
         renderer.GetPropertyBlock(_dissolvePropertBlock);
         _dissolvePropertBlock.SetFloat("_CutofffHeight", _dissolveMaxHeight);
         renderer.SetPropertyBlock(_dissolvePropertBlock);
      }
   }
}



