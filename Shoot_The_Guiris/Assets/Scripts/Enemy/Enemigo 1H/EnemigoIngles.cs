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
   [Header("Configuracion")]
   Transform _target;
   [SerializeField] string _targetTag = "Player";
   [Header("Attack")]
   [SerializeField] float _attackDistance;

   [SerializeField] float _inRange;
   public bool AgentIsActive => _agent.enabled;
   public bool HasTarget => _target != null;
   public float RemainingDistanceToTarget => _agent.remainingDistance;
   public bool PathPending => _agent.pathPending;
   public float AttacDistance => _attackDistance;
   public float InRange => _inRange;

   public Transform Target => _target;
   public override void EditorInit()
   {
      base.EditorInit();
      _agent = GetComponent<NavMeshAgent>();
      _animator = GetComponentInChildren<Animator>();
   }

   void Start()
   {
      CheckForTarget(_targetTag);
   }

   void Update()
   {
      AnimationController();
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



