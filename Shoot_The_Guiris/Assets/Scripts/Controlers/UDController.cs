using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UDController: CustomMonoBehaviour
{
   [SerializeField] Image _barradeVida;
   [SerializeField] CanvasGroup _canvasGroupdeDanio;
   [SerializeField] float _velocidadFlash = 1f;
   private Coroutine _coroutinaDanio;
   public override void EditorInit()
   {

   }

   void Start()
   {

   }

   void Update()
   {

   }

   private void SetHealth(float vidaActual, float vidaMaxima)
   {
      _barradeVida.fillAmount = vidaActual / vidaMaxima;
   }
      

   private void FlashDanio()
   {
      if(_coroutinaDanio != null)
      {
         StopCoroutine(_coroutinaDanio);
      }
      _coroutinaDanio = StartCoroutine(FlashDanioCoroutine());
   }
      

   private IEnumerator FlashDanioCoroutine()
   {
      _canvasGroupdeDanio.alpha = 1f;
      float _timer = 0f;
      while(_timer < _velocidadFlash)
      {
         float t =1 - (_timer / _velocidadFlash);
         _canvasGroupdeDanio.alpha = Mathf.Lerp(1f, 0f, t);
         _timer += Time.deltaTime;
         yield return null;
      }
      _canvasGroupdeDanio.alpha = 0f;
   }
}

   

   