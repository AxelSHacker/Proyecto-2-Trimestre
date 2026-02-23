using System;
using UnityEngine;

public class GameController : CustomMonoBehaviour
{
   [SerializeField] PlayerControler _playerController;
   [SerializeField] HUDController _hudController;
   [SerializeField] WaveController _waveController;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      _playerController.AddObservable(_hudController);

   }

   void Start()
   {
      _waveController.StartWave();
   }

   void Update()
   {

   }
}



