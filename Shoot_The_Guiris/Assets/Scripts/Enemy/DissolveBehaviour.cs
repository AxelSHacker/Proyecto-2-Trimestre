using Unity.VisualScripting;
using UnityEngine;

public class DissolveBehaviour : CustomMonoBehaviour
{
   [SerializeField] Renderer _renderer;
   MaterialPropertyBlock _dissolvePropertBlock;
   [SerializeField] float _dissolveTiime = 1f;
   [SerializeField] float _dissolveMaxHeight = 2f;
   [SerializeField] float _dissolveMinHeight = -2f;
   float _timer;
   float _currentrHeight;
   public override void EditorInit()
   {
      _renderer = GetComponentInChildren<Renderer>();
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

   }
}



