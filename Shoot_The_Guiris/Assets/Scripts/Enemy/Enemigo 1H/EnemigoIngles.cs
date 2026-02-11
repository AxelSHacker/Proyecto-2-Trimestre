using System.Collections;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemigoIngles : PoolEntity
{
   [Header("Referencias")]
   [SerializeField] NavMeshAgent _agent;
   [SerializeField] GameObject _weapon;
   [SerializeField] Animator _animator;
   [SerializeField] Rigidbody _rB;
   [Header("Configuracion")]
   Transform _target;
   
   [SerializeField] string _targetTag = "Player";
   [Header("GroundCheck")]
   [SerializeField] LayerMask _groundLayer;
   [SerializeField] Transform _groundCheckPoint;
   [SerializeField] float _groundCheckSize;
   [SerializeField] bool _grounded;
   bool _volando;
   [Header("Coroutine")]
   Coroutine _levantarse;
   [Header("Attack")]
   [SerializeField] float _attackDistance;
   [SerializeField] float _inRange;
   public bool AgentIsActive => _agent.enabled;
   public bool HasTarget => _target != null && _grounded && _agent.enabled;
   public float RemainingDistanceToTarget => _agent.enabled ? _agent.remainingDistance : 0f;
   public bool PathPending => _agent.enabled && _agent.pathPending && _grounded;
   public float AttacDistance => _attackDistance;
   public float InRange => _inRange;


   public Transform Target => _target;
   public override void EditorInit()
   {
      base.EditorInit();
      _agent = GetComponent<NavMeshAgent>();
      _animator = GetComponentInChildren<Animator>();
      _rB = GetComponent<Rigidbody>();
   }


   void Start()
   {
      CheckForTarget(_targetTag);
   }

   void Update()
   {
      GroundCheck();
      if (_volando && _grounded && _rB.linearVelocity.y <= 0.1f)
      {
         _volando = false;
         if ( _levantarse != null) StopCoroutine(_levantarse);
         _levantarse = StartCoroutine(RutinaLevantarse());
      }
      AnimationController();

   }




   void OnDrawGizmos()
   {
      //Cambiamos el color del Gizmos
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckSize);
   }



   private void GroundCheck()
   {
      //Solo vamos a comprobar si es mayor que 0, asi que no necesitamos mas capacida de buffer
      Collider[] colliderBuffer = new Collider[1];
      //Comprobamos si hay contacto con el suelo, lo hacemos mediant un OvrlapboxnonAlloc,
      //para no consumir mas memoria de la necesaria ya que esta funcion la vamos a hacer de manera continua
      Physics.OverlapSphereNonAlloc(_groundCheckPoint.position, _groundCheckSize, colliderBuffer, _groundLayer);
      //Actualitzamos el estado de _grounded
      _grounded = colliderBuffer[0] != null;
      
   }
   public void ImpactoViento(Vector3 direccion)
   {
      //Si volamos cancelamos la recuperacion
      if (_levantarse != null) StopCoroutine(_levantarse);
      _volando = true;
      _agent.enabled = false;
      _rB.isKinematic = false;
      _rB.constraints = RigidbodyConstraints.None;
      _rB.AddForce(direccion, ForceMode.Impulse);

   }

   private IEnumerator RutinaLevantarse()
   {
      yield return new WaitForSeconds(Random.Range(1f, 3f));
      //Lo rotamos a posicion normal
      transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
      //Volvemos a estado grounded
      _rB.isKinematic = true;
      _agent.enabled = true;
      _agent.Warp(transform.position);
      _levantarse = null;
   }
   //Buscamos el objeto mas cercano con el ltag indicado
   public void CheckForTarget(string name)
   {
      GameObject[] possibleTarget = GameObject.FindGameObjectsWithTag(_targetTag);


      if (possibleTarget == null || possibleTarget.Length == 0) return;
      _target = possibleTarget[0].transform;
      float minDistance = Vector3.Distance(_target.position, transform.position);

      for (int i = 1; i < possibleTarget.Length; i++)
      {
         float distance = Vector3.Distance(possibleTarget[i].transform.position, transform.position);
         if (distance < minDistance)
         {
            _target = possibleTarget[i].transform;
            minDistance = distance;
         }
      }
   }
   public void SetDestination(Vector3 destinationPoint)
   {
      _agent.SetDestination(destinationPoint);
   }

   public void SetDestinationToTarget()
   {
      SetDestination(_target.position);
   }
   private void AnimationController()
   {
      if (_agent.velocity.sqrMagnitude > 0.1f)
      {
         _animator.SetFloat("Velocidad", _agent.velocity.magnitude);
      }
      else
      {
         _animator.SetFloat("Velocidad", 0);
      }
   }

}



