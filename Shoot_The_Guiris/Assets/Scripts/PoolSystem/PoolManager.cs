using Unity.VisualScripting;
using UnityEngine;

public class PoolManager : CustomMonoBehaviour
{
   private static PoolManager _instance;
   public static PoolManager Instance => _instance;
   [SerializeField] Pool[] pools;
   public override void EditorInit()
   {

   }
   void Awake()
   {
      if (_instance == null)
      {
         _instance = this;

      }
      else
      {
         Destroy(this);
      }

   }
   void Start()
   {

   }

   void Update()
   {

   }
}



