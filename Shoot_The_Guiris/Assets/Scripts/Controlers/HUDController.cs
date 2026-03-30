
using System.Collections;
using TMPro;

using UnityEngine;
using UnityEngine.UI;

public class HUDController : CustomMonoBehaviour, PlayerObserver
{
   #region Variables
   [SerializeField] Image _barradeVida;
   [SerializeField] CanvasGroup _canvasGroupdeDanio;
   [SerializeField] float _velocidadFlash = 1f;
   private Coroutine _coroutinaDanio;
   [SerializeField] Image _ataqueEspecial;
   [SerializeField] Image[] _dash;
   [SerializeField] Animator _wavePanel;
   [SerializeField] TextMeshProUGUI _waveText;

   #endregion
   public override void EditorInit()
   {

   }

   void Start()
   {
      _ataqueEspecial.fillAmount = 1f;
   }

   void Update()
   {

   }
   #region Funciones
   private void SetHealth(float vidaActual, float vidaMaxima)
   {
      _barradeVida.fillAmount = Mathf.Clamp01((float)vidaActual / vidaMaxima);
   }

   private void UpdateAtaqueEspecial(float timer, float time)
   {
      _ataqueEspecial.fillAmount = 1f - (timer / time);
   }
   private void UpdateDash(float timer, float time)
   {
      for (int i = 0; i < _dash.Length; i++)
      {
         _dash[i].fillAmount = 1f - (timer / time);
      }
   }
   public void ShowWavePanel(int waveNumber)
   {
      
      _waveText.text = waveNumber.ToString();
      _wavePanel.SetTrigger("Oleada");
   }
   private void FlashDanio()
   {
      if (_coroutinaDanio != null)
      {
         StopCoroutine(_coroutinaDanio);
      }
      _coroutinaDanio = StartCoroutine(FlashDanioCoroutine());
   }
   #endregion

   #region Coroutine
   private IEnumerator FlashDanioCoroutine()
   {
      _canvasGroupdeDanio.alpha = 1f;
      float _timer = 0f;
      while (_timer < _velocidadFlash)
      {
         float t = 1 - (_timer / _velocidadFlash);
         _canvasGroupdeDanio.alpha = Mathf.Lerp(1f, 0f, t);
         _timer += Time.deltaTime;
         yield return null;
      }
      _canvasGroupdeDanio.alpha = 0f;
   }
   #endregion
   #region  IdamagableObserver
   public void OnHealtUpdate(float currentealt, float maxHealt)
   {
      SetHealth(currentealt, maxHealt);

   }
   public void OnHit()
   {
      FlashDanio();
   }
   public void OnDead()
   {

   }

   public void OnAtaqueEspecial(float timer, float time)
   {
      UpdateAtaqueEspecial(timer, time);
   }

   public void OnDasch(float timer, float time)
   {
      UpdateDash(timer, time);
   }
   #endregion
}



