using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : CustomMonoBehaviour
{
   [SerializeField] PlayerControler _playerController;
   [SerializeField] HUDController _hudController;
   [SerializeField] WaveController _waveController;
   [SerializeField] List<GameObject> _walls;
   [SerializeField] CanvasGroup _menuVictoria;

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
      BlockingWalls.OnWallDestroy += PuertaDestruida;
   }
   void OnDisable()
   {
      _waveController.OnWaveStart -= WaveHasStarted;
      _waveController.OnWaveEnd -= WaveHasEnded;
      BlockingWalls.OnWallDestroy -= PuertaDestruida;
   }
   void Start()
   {
      //_waveController.StartWave();
      _esperandoSiguienteOleada = false;
      _siguienteOleadaTimer = Time.time + _timeEntreOleadas;
   }
   void Update()
   {
      if (_waveController.tutorial) return;
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
   
   public void ExitButton()
   {
      SceneManager.LoadScene("Main Menu");
   }
   private void PuertaDestruida(GameObject wall)
   {
      if (_walls.Contains(wall))
      {
         _walls.Remove(wall);
      }
      if (_walls.Count == 0)
      {
         Time.timeScale = 0f;
         _menuVictoria.alpha = 1f;
         _menuVictoria.interactable = true;
         _menuVictoria.blocksRaycasts = true;
      }
   }
   
}











