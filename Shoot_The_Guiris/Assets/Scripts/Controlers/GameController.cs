using UnityEngine;

public class GameController : CustomMonoBehaviour
{
   [SerializeField] PlayerControler _playerController;
   [SerializeField] HUDController _hudController;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      _playerController.AddObservable(_hudController);

   }

   void Start()
   {

   }

   void Update()
   {

   }
}



