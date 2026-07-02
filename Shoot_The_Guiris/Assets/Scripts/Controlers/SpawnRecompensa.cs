
using System;
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
   [SerializeField] float _tamanioArea = 0.5f;
   [SerializeField] float _fuerzaMagnetica = 10f;
   [SerializeField] LayerMask _recogibleLayers;
   private Collider[] colliders = new Collider[3];

   [SerializeField] int _cantidadMunicionARecargar = 20;

   public override void EditorInit()
   {
      base.EditorInit();
   }

   void Update()
   {
      if (!IsActive) return;

      // CheckContacto();
      Magnetizacion();
   }
   void OnDrawGizmos()
   {
      Gizmos.color = Color.yellow;
      Gizmos.DrawWireSphere(transform.position, _tamanioArea);
   }
   void OnTriggerEnter(Collider other)
   {
      if (!IsActive) return;

      if (((1 << other.gameObject.layer) & _recogibleLayers) != 0)
      {
         AplicarRecompensa(other.gameObject);
         ReturnToPool();
      }
   }
   private void Magnetizacion()
   {
      // Buscamos si el Player entra en el radio del imán
      int cantidad = Physics.OverlapSphereNonAlloc(transform.position, _tamanioArea, colliders, _recogibleLayers);

      if (cantidad > 0)
      {
         // Como tu capa '_recogibleLayers' busca al Player, el colliders[0] será el jugador
         Transform playerTransform = colliders[0].transform;

         // 1. Calculamos la dirección hacia el jugador (encontrando la altura de su centro/cuerpo)
         Vector3 posicionObjetivo = playerTransform.position;

         // TRUCO ANTI-SUELO: Mantenemos la altura (Y) original del reward para que no intente 
         // clavarse en los pies del jugador y picar hacia abajo atravesando el mapa.
         posicionObjetivo.y = transform.position.y;

         // 2. Movemos el objeto suavemente hacia el jugador usando Lerp o MoveTowards
         // Esto ignora las fuerzas físicas caóticas y da un control del 100% en Android
         transform.position = Vector3.MoveTowards(transform.position, posicionObjetivo, _fuerzaMagnetica * Time.deltaTime);
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
            //Logica para ganar dinero
            player.GetComponent<PlayerControler>().RecibirDinero();
            break;
         case TipoRecompensa.Ammo:
            // Lógica para dar munición al jugador
            player.GetComponent<PlayerControler>().RecibirMunicion(_cantidadMunicionARecargar);
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
      SpawnRandomRecompensa();
   }
   public override void Deactivate()
   {
      base.Deactivate();
   }
   #endregion













}



