using System;
using UnityEngine;

public class GameController : CustomMonoBehaviour
{
   [SerializeField] PlayerControler _playerController;
   [SerializeField] HUDController _hudController;
   [SerializeField] WaveController _waveController;

   [SerializeField] float _timeEntreOleadas = 5f;
   float _siguienteOleadaTimer;
   bool _esperandoSiguienteOleada;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      _playerController.AddObservable(_hudController);

   }
   void OnEnable()
   {
      _waveController.OnWaveStart += WaveHasStarted;
      _waveController.OnWaveEnd += WaveHasEnded;
   }


   void OnDisable()
   {
      _waveController.OnWaveStart -= WaveHasStarted;
      _waveController.OnWaveEnd -= WaveHasEnded;
   }

   void Start()
   {
      //_waveController.StartWave();
      _esperandoSiguienteOleada = false;
      _siguienteOleadaTimer = Time.time + _timeEntreOleadas;
   }

   void Update()
   {
      if (_esperandoSiguienteOleada) return;

      if (Time.time >= _siguienteOleadaTimer)
      {

         _waveController.StartWave();
         _esperandoSiguienteOleada = true;
      }
   }
   private void WaveHasEnded(int waveNumber)
   {
      _esperandoSiguienteOleada = false;
      _siguienteOleadaTimer = Time.time + _timeEntreOleadas;
   }

   private void WaveHasStarted(int waveNumber)
   {
      _hudController.ShowWavePanel(waveNumber);
   }
}



