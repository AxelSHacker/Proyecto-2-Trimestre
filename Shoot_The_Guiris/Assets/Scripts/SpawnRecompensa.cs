using Unity.Mathematics;
using UnityEngine;

public class SpawnRecompensa : PoolEntity
{
   public enum TipoRecompensa
   {
      Coin,
      Ammo,
      Health
   }
   [System.Serializable]
   public struct RecompensaData
   {
      public TipoRecompensa tipo;
      public ParticleSystem objeto;
   }

   [SerializeField] RecompensaData[] _recompensas;
   TipoRecompensa _tipoRecompensa;
   Rigidbody _rigidbody;
   [SerializeField] Vector3 _tamanioCaja = new Vector3(0.5f, 0.5f, 0.5f);
   [SerializeField] Vector3 _offset = new Vector3(0f, 0f, 1f);
   [SerializeField] LayerMask _recogibleLayers;
   private Collider[] colliders = new Collider[3];


   public override void EditorInit()
   {
      base.EditorInit();
      _rigidbody = GetComponent<Rigidbody>();

   }

   void Update()
   {
      if (!IsActive) return;

      CheckContacto();
   }
   void OnDrawGizmos()
   {
      Matrix4x4 rotationMatrix = Matrix4x4.TRS(transform.TransformPoint(_offset), transform.rotation, _tamanioCaja);
      Gizmos.matrix = rotationMatrix;
      Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
   }

   private void CheckContacto()
   {
      Vector3 centro = transform.TransformPoint(_offset);

      int cantidad = Physics.OverlapBoxNonAlloc(centro, _tamanioCaja / 2, colliders ,transform.rotation, _recogibleLayers);
      
      if (cantidad > 0)
      {
         AplicarRecompensa(colliders[0].gameObject);
         ReturnToPool();
      }
   }
   private void SpawnRandomRecompensa()
   {
      int index = UnityEngine.Random.Range(0, _recompensas.Length);
      for (int i = 0; i < _recompensas.Length; i++)
      {
         bool isSelected = (i == index);
         if (isSelected)
         {
            _tipoRecompensa = _recompensas[i].tipo;
            _recompensas[i].objeto.gameObject.SetActive(true);

            _recompensas[i].objeto.Clear();
            _recompensas[i].objeto.Play(true);
         }
         else
         {
            _recompensas[i].objeto.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _recompensas[i].objeto.gameObject.SetActive(false);
         }
      }

   }
   private void AplicarRecompensa(GameObject player)
   {
      switch (_tipoRecompensa)
      {
         case TipoRecompensa.Coin:
            player.GetComponent<PlayerControler>().RecibirDinero(100);
            break;
         case TipoRecompensa.Ammo:
            player.GetComponent<PlayerControler>().RecibirMunicion(10, "ProyectilCaca");
            player.GetComponent<PlayerControler>().RecibirMunicion(5, "Misil");
            // Lógica para dar munición al jugador
            break;
         case TipoRecompensa.Health:
            player.GetComponent<PlayerControler>().RecibirVida(10);
            // Lógica para curar al jugador
            break;
      }
   }
   #region PoolEntity
   public override void Initialize()
   {
      base.Initialize();

      if (_rigidbody != null)
      {
         _rigidbody.isKinematic = false;

         _rigidbody.linearVelocity = Vector3.zero;
         _rigidbody.angularVelocity = Vector3.zero;

         _rigidbody.AddForce(Vector3.up * 5f, ForceMode.Impulse);
         Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 10f;

         _rigidbody.AddForce(randomDirection, ForceMode.Impulse);

      }
      SpawnRandomRecompensa();
   }
   public override void Deactivate()
   {
      base.Deactivate();
   }
   #endregion













}



