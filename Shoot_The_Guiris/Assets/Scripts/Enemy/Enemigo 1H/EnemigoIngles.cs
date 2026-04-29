
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;


public class EnemigoIngles : PoolEntity, IDamageabe<float>, PlayerObserver, IObservable<PlayerObserver>
{
   #region Variables
   [Header("Referencias")]
   [SerializeField] NavMeshAgent _agent;
   [SerializeField] Transform _weapon;
   [SerializeField] Transform _weapon2;
   [SerializeField] Animator _animator;
   [SerializeField] Rigidbody _rB;
   [SerializeField] RandoSoundEffecs _randoSoundEffects;

   Collider[] colliderBuffer = new Collider[1];

   [Header("Configuracion")]
   [SerializeField] Transform _target;
   [SerializeField] string _targetTag = "Player";

   [Header("Reward")]
   [SerializeField] string _rewardPoolID;
   [SerializeField] float _rewardChance = 150f;

   [Header("GroundCheck")]
   [SerializeField] LayerMask _groundLayer;
   [SerializeField] Transform _groundCheckPoint;
   [SerializeField] float _groundCheckSize;
   [SerializeField] bool _grounded;
   bool _volando = false;
   bool _cegado = false;
   [SerializeField] float timetoDeactivate;
   [SerializeField] float _maxTimeToDeactivate;

   [Header("Coroutine")]
   Coroutine _levantarse;
   Coroutine _ralentizacion;
   Coroutine _ceguera;
   Coroutine _ataqueSalto;

   [Header("Movimiento")]
   float _actualSpeed;

   [Header("Attack")]
   [SerializeField] float _attackDistance;
   [SerializeField] float _inRange;
   [SerializeField] float _maxhealth;
   [SerializeField] float _currentealth;
   [SerializeField] int _cargadorMax;
   [SerializeField] float _velocidadAtaque;

   [Header("Anti-Atasco")]
   [SerializeField] float _timeToDieIfStuck = 10f; // Tiempo límite
   [SerializeField] float _stuckDistanceThreshold = 0.1f; // Distancia mínima que debe recorrer
   float distanceMoved;

   private Vector3 _lastPosition;
   private float _stuckTimer;
   float _stuckCheckTimer;
   int _cargador;
   #endregion
   [Header("Getters")]
   #region Getters
   public bool AgentIsActive => _agent.enabled;
   public bool HasTarget => _target != null && _agent.enabled;
   public float RemainingDistanceToTarget => _agent.enabled ? _agent.remainingDistance : 0f;
   public bool PathPending => _agent.enabled && _agent.pathPending;
   public float AttacDistance => _attackDistance;
   public float InRange => _inRange;
   public Transform Target => _target;
   public float Maxhealt { get => _maxhealth; }
   public float Currentealt { get => _currentealth; }
   public bool IsDead => _currentealth <= 0;
   public Transform Posicion => _weapon;
   public Transform Posicion2 => _weapon2;
   public float VelocidadAtaque => _velocidadAtaque;
   public bool Ciego => _cegado;
   #endregion


   public UnityEvent OnInizialize;
   public UnityEvent OnDeactivate;
   public UnityEvent OnDeadUE;
   #region Start/Update
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
   
      AutomaticDeactivation();

      AnimationController();
   }

   void OnDrawGizmos()
   {
      //Cambiamos el color del Gizmos
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckSize);
   }
   void OnCollisionEnter(Collision collision)
   {
      if (_volando)
      {
         if (((1 << collision.gameObject.layer) & _groundLayer) != 0)
         {
            _volando = false;
            _rB.linearVelocity = Vector3.zero;

            if (_levantarse != null) StopCoroutine(_levantarse);
            _levantarse = StartCoroutine(RutinaLevantarse());
         }
      }
   }
   #endregion




   #region Funciones
   private void GroundCheck()
   {
      //Solo vamos a comprobar si es mayor que 0, asi que no necesitamos mas capacida de buffer

      //Comprobamos si hay contacto con el suelo, lo hacemos mediant un OvrlapboxnonAlloc,
      //para no consumir mas memoria de la necesaria ya que esta funcion la vamos a hacer de manera continua
      Physics.OverlapSphereNonAlloc(_groundCheckPoint.position, _groundCheckSize, colliderBuffer, _groundLayer);
      //Actualitzamos el estado de _grounded
      _grounded = colliderBuffer[0] != null;
   }
   //Funcuion que se llama desde la municion de viento, para lanzar al enemigo por los aires
   public void ImpactoViento(Vector3 direccion)
   {
      _animator.SetBool("EnRango", false);
      //Si volamos cancelamos la recuperacion
      if (_levantarse != null) StopCoroutine(_levantarse);
      _animator.SetLayerWeight(1, 0);
      _volando = true;
      _agent.enabled = false;
      _rB.isKinematic = false;
      _rB.AddForce(direccion, ForceMode.Impulse);
   }
   //Funcion que se llama desde el ataque especial, para lanzar al enemigo por los aires
   //Funcion que llamamos desde la municion de tinta, para cegar al enemigo
   public void Cegar()
   {
      if (_volando || !_grounded) return;
      if (_ceguera != null) StopCoroutine(_ceguera);
      _ceguera = StartCoroutine(Ceguera());
   }
   //Funcion que llamamos desde la municion de caca, para ralentizar al enemigo
   public void RaletizacionCoroutina()
   {
      if (_ralentizacion != null) StopCoroutine(_ralentizacion);
      _ralentizacion = StartCoroutine(Ralentizacion());
   }
   public void AtaqueSaltoCoroutina()
   {
      if (_ataqueSalto != null) StopCoroutine(_ataqueSalto);
      _ataqueSalto = StartCoroutine(AtaqueSalto());
   }
   public void DisparoRealizado()
   {
      _cargador--;
      if (_cargador <= 0)
      {
         _animator.SetTrigger("Recarga");
      }
   }
   public void RecargaRealizada()
   {
      _cargador = _cargadorMax;
   }
   private void SpameoReward()
   {
      if (Random.Range(0f, 100f) <= _rewardChance)
      {
         Vector3 position = transform.position + Vector3.up * 3f; // Ajusta la altura según sea necesario
         PoolManager.Instance.Pull(_rewardPoolID, position, Quaternion.identity);
      }
   }
   //Buscamos el objeto mas cercano con el tag indicado
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
   private void AutomaticDeactivation()
   {
      if (IsDead) return;
      bool agentError = _agent.enabled && !_agent.isOnNavMesh;

      if ((_volando || agentError) && !_grounded)
      {
         timetoDeactivate -= Time.deltaTime;

         if (timetoDeactivate <= 0f)
         {
            Death();
         }

      }
      else if (_grounded && agentError)
      {
         Death();
      }
      else
      {
         timetoDeactivate += _maxTimeToDeactivate;
      }
   }

   public void Revivir()
   {
      timetoDeactivate = _maxTimeToDeactivate;

      _currentealth = _maxhealth;

      _animator.SetLayerWeight(1, 1);

      _agent.speed = Random.Range(30f, 35f);

      _actualSpeed = _agent.speed;

      _cargador = _cargadorMax;

      _lastPosition = transform.position;

      _cegado = false;

      _animator.Rebind();

   }
   private void Death()
   {
      _agent.isStopped = true;

      int tipoMuerte = Random.Range(1, 4);

      _animator.SetInteger("Muerte", tipoMuerte);

      _animator.SetLayerWeight(1, 0);

      _volando = false;

      SpameoReward();

      for (int i = 0; i < _observers.Count; i++)
      {
         _observers[i].OnDead();
      }
      _observers.Clear();
   }
   #endregion



   #region Coroutines
   public IEnumerator ImpactoPatada(Vector3 direccion)
   {
      _agent.enabled = false;
      _rB.isKinematic = false;
      _animator.SetLayerWeight(1, 0);
      _rB.AddForce(direccion, ForceMode.Impulse);
      yield return new WaitForSeconds(3f);
      _animator.SetLayerWeight(1, 1);
      _agent.enabled = true;
      _rB.isKinematic = true;
      _agent.Warp(transform.position);
   }
   //Coroutina que ralentiza al enemigo cuando es golpeado por la municion de caca
   private IEnumerator Ralentizacion()
   {
      _agent.speed = _actualSpeed * 0.5f;
      yield return new WaitForSeconds(1f);
      _agent.speed = _actualSpeed;
      _ralentizacion = null;
   }
   //Coroutina que se encarga de levantar al enemigo despues de ser lanzado por los aires
   private IEnumerator RutinaLevantarse()
   {
      yield return new WaitForSeconds(Random.Range(2f, 5f));
      NavMeshHit hit;
      if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
      {
         //Lo rotamos a posicion normal
         transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
         //Volvemos a estado grounded
         _animator.SetLayerWeight(1, 1);
         //Devolvemos los componentes a su estado de persecucion
         _rB.isKinematic = true;
         _agent.enabled = true;
         _agent.Warp(hit.position);
      }
      else
      {
         Death();
      }

      _levantarse = null;
   }
   //Coroutine de ceguera, el enemigo se mueve de manera erratica durante 3 segundos, y luego vuelve a la normalidad
   private IEnumerator Ceguera()
   {
      _cegado = true;
      _agent.speed = _actualSpeed * 0.5f;
      // 1. Calculamos un punto cercano relativo a donde está AHORA
      Vector3 desplazamiento = Random.insideUnitSphere * 10f;
      desplazamiento.y = 0;
      Vector3 pAleatorio = transform.position + desplazamiento;

      NavMeshHit hit;
      // Buscamos un punto válido en el NavMesh
      if (NavMesh.SamplePosition(pAleatorio, out hit, 10f, NavMesh.AllAreas))
      {
         _agent.SetDestination(hit.position);
         Debug.Log("Cegado: Caminando a nuevo punto aleatorio");
      }
      yield return new WaitForSeconds(2f);

      _agent.speed = _actualSpeed;
      _cegado = false;
   }
   //Coroutina que se encarga de realizar el ataque con salto del Boos
   private IEnumerator AtaqueSalto()
   {
      float tiempoSalto = 1.10f;
      float timer = 0f;
      float alturaMaxima = 20f;

      _agent.enabled = false;
      if (_animator.layerCount > 1) _animator.SetLayerWeight(1, 0f);

      Vector3 posInicio = transform.position;
      Vector3 posicionCaida = _target.position - new Vector3(1f, 0f, 1f);
      Vector3 posFinal = posicionCaida;

      while (timer < tiempoSalto)
      {
         timer += Time.deltaTime;
         float t = timer / tiempoSalto; // 0 a 1 lineal

         //CALCULO DEL AVANCE HORIZONTAL (Suave)
         float tSuave = t * t * (3f - 2f * t);
         Vector3 posHorizontalActual = Vector3.Lerp(posInicio, posFinal, tSuave);

         // CALCULO DEL ARCO (Parábola pura)
         // Esta fórmula asegura que en t=0 es 0, en t=0.5 es alturaMaxima, y en t=1 es 0
         float arcoY = 4f * alturaMaxima * t * (1f - t);


         //La Y es la interpolación del suelo + el arco
         float yFinal = Mathf.Lerp(posInicio.y, posFinal.y, tSuave) + arcoY;

         // 4. APLICAR DIRECTO AL TRANSFORM
         transform.position = new Vector3(posHorizontalActual.x, yFinal, posHorizontalActual.z);
         Debug.DrawLine(transform.position, transform.position + Vector3.up * 2f, Color.green, 0.1f);
         yield return null;
      }

      // Finalización limpia
      transform.position = posFinal;
      _agent.enabled = true;
      _agent.Warp(transform.position);
      if (_animator.layerCount > 1) _animator.SetLayerWeight(1, 1f);
      _ataqueSalto = null;
   }
   #endregion



   #region Animaciones Evetns
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

      _animator.SetBool("Grounded", _grounded);
   }

   public void AnimatorDeactivate()
   {
      _animator.enabled = false;
   }
   #endregion




   #region PooEntity
   public override void Initialize()
   {
      base.Initialize();

      _animator.enabled = true;

      _agent.enabled = true;

      _agent.Warp(transform.position);

      _agent.isStopped = false;

      _agent.ResetPath();

      OnInizialize?.Invoke();
   }



   public override void Deactivate()
   {
      base.Deactivate();

      StopAllCoroutines();
      _animator.enabled = false;
      _levantarse = null;
      _ralentizacion = null;
      OnDeactivate?.Invoke();
   }
   #endregion




   #region Herencias
   public void TakeDamag(float damage, Vector3 impactPoint = default)
   {
      if (IsDead) return;
      _currentealth -= damage;
      _currentealth = Mathf.Clamp(_currentealth, 0, _maxhealth);


      for (int i = 0; i < _observers.Count; i++)
      {
         _observers[i].OnHealtUpdate(_currentealth, _maxhealth);
         _observers[i].OnHit();
      }
      _randoSoundEffects.PlayRandomSoundEffect();
      if (_currentealth <= 0)
      {
         Death();
      }
   }
   public void OnHealtUpdate(float currentealt, float maxealt)
   {

   }
   public void OnHit()
   {
   }
   public void OnDead()
   {

   }
   public void OnAtaqueEspecial(float timer, float time)
   {

   }

   public void OnDasch(float timer, float time)
   {

   }
   #endregion




   #region IObservable implementation
   private List<PlayerObserver> _observers = new List<PlayerObserver>();
   public void AddObservable(PlayerObserver observable)
   {
      if (_observers == null) _observers = new List<PlayerObserver>();

      _observers.Add(observable);
   }

   public void RemoveObservable(PlayerObserver observable)
   {
      if (_observers == null) _observers = new List<PlayerObserver>();

      _observers.Remove(observable);
   }
   #endregion
}























































