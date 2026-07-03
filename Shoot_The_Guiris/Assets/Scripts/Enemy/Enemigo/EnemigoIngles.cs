
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
   internal bool _volando = false;
   [SerializeField] float _timetoDeactivate;
   [SerializeField] float _maxTimeToDeactivate;

   [Header("Coroutine")]
   Coroutine _ralentizacion;
   Coroutine _ataqueSalto;

   [Header("Movimiento")]
   float _actualSpeed;
   float _velocidadBusqueda;
   float _velocidadEnRango; 
   internal Vector3 _destino1;
   internal Vector3 _destino2;
   internal Vector3 _destinoFinal;

   [Header("Attack")]
   [SerializeField] float _attackDistance;
   [SerializeField] float _inRange;
   [SerializeField] float _maxhealth;
   [SerializeField] float _currentealth;
   [SerializeField] int _cargadorMax;
   [SerializeField] float _velocidadAtaque;

   int _cargador;
   #endregion
   [Header("Getters")]
   #region Getters
   public NavMeshAgent Agent => _agent;
   public Rigidbody Rigidbody => _rB;
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
   public float VelocidadBusqueda => _velocidadBusqueda;
   public float VelocidadEnRango => _actualSpeed;
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
      _velocidadBusqueda = _actualSpeed * 1.5f;
   }
   void Update()
   {
      if (!IsActive || IsDead) return;
      GroundCheck();
      AnimationController();
   }
   void OnDrawGizmos()
   {
      //Cambiamos el color del Gizmos
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckSize);
   }
   #endregion




   #region Funciones
   private void GroundCheck()
   {
      //Comprobamos si hay contacto con el suelo, lo hacemos mediant un OvrlapboxnonAlloc,
      //para no consumir mas memoria de la necesaria ya que esta funcion la vamos a hacer de manera continua
      int colliderCount = Physics.OverlapSphereNonAlloc(_groundCheckPoint.position, _groundCheckSize, colliderBuffer, _groundLayer);
      //Actualitzamos el estado de _grounded
      _grounded = colliderCount > 0;
   }
   //Funcuion que se llama desde la municion de viento, para lanzar al enemigo por los aires
   public void ImpactoViento(Vector3 direccion)
   {
      _agent.enabled = false;
      _rB.isKinematic = false;
      _rB.AddForce(direccion, ForceMode.Impulse);
   }
   public void ImpactoPatada(Vector3 direccion)
   {
      _agent.enabled = false;
      _rB.isKinematic = false;
      //Si volamos cancelamos la recuperacion
      _rB.AddForce(direccion, ForceMode.Impulse);
   }
   public void Levantarse()
   {
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
   }
   //Funcion que se llama desde el ataque especial, para lanzar al enemigo por los aires
   //Funcion que llamamos desde la municion de tinta, para cegar al enemigo
   public void Cegar()
   {
      _animator.SetTrigger("Ciego");
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
      if (!_grounded) return;
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
   public void AutomaticDeactivation()
   {
      bool agentError = _agent.enabled && !_agent.isOnNavMesh;

      if ((_volando || agentError) && !_grounded)
      {
         _timetoDeactivate -= Time.deltaTime;

         if (_timetoDeactivate <= 0f)
         {
            Death();
         }

      }
      else if (_grounded && agentError)
      {
         Death();
      }
   }
   public void Revivir()
   {
      _timetoDeactivate = _maxTimeToDeactivate;

      _agent.isStopped = false;

      _currentealth = _maxhealth;

      _animator.SetLayerWeight(1, 1);

      _agent.speed = Random.Range(30f, 35f);

      _actualSpeed = _agent.speed;

      _cargador = _cargadorMax;

      _animator.Rebind();

   }
   public void CalculoZonaBusqueda1()
   {
      // Elegimos un ángulo aleatorio en grados y lo convertimos a Radianes multiplicando por Deg2Rad
      float randomAngle1 = Random.Range(-90f, 90f) * Mathf.Deg2Rad;
      // Convertimos el ángulo en un vector de dirección local (X, Z) usando las funciones trigonométricas Sin y Cos
      Vector3 direction1 = new Vector3(Mathf.Sin(randomAngle1), 0, Mathf.Cos(randomAngle1));
      // Orientamos esa dirección local según hacia dónde estaba mirando el enemigo en el mundo real
      Vector3 worldPos1 = transform.TransformDirection(direction1);
      // Elegimos una distancia al azar entre 5 metros y el radio máximo permitido
      float randomDistance = Random.Range(5f, 20f);
      // Sumamos la dirección escalada por la distancia a nuestro centro de búsqueda para obtener las coordenadas finales
      _destino1 = transform.position + (worldPos1 * randomDistance);
   }
   // Calcula un punto al azar en la mitad trasera del enemigo (90° a 270°)
   public void CalculoZonaBusqueda2()
   {
      // El proceso es idéntico, pero abriendo el cono hacia la espalda del enemigo
      float randomAngle2 = Random.Range(90f, 270f) * Mathf.Deg2Rad;

      Vector3 direction2 = new Vector3(Mathf.Sin(randomAngle2), 0, Mathf.Cos(randomAngle2));
      Vector3 worldPos2 = transform.TransformDirection(direction2);
      float randomDistance = Random.Range(3f, 10f);

      _destino2 = transform.position + (worldPos2 * randomDistance);
   }
   public bool ComprobarZonas(Vector3 pos)
   {
      NavMeshHit navMeshHit;
      //Proyectamos el punto matematicoen el navMesh en un radio de 4 metros
      //Si encuentra suelo transitable guarda la posicion exacta pegada al suelo en el navmeshhit
      if (NavMesh.SamplePosition(pos, out navMeshHit, 5.0f, NavMesh.AllAreas))
      {
         NavMeshPath path = new NavMeshPath();
         //Le pedimos al agente que calcule la ruta en silencio desde donde esta hasta el suelo del navmesh
         _agent.CalculatePath(navMeshHit.position, path);
         //Si la ruta esta completa es decir, no hay interferencias
         if (path.status == NavMeshPathStatus.PathComplete)
         {
            //el destino es seguro y lo guardamos para darselo al agent
            _destinoFinal = navMeshHit.position;
            return true;
         }
      }
      return false;
   }
   private void Death()
   {

      int tipoMuerte = Random.Range(1, 4);

      if (_agent.enabled) _agent.isStopped = true;

      _animator.SetInteger("Muerte", tipoMuerte);

      _animator.SetLayerWeight(1, 0);

      _volando = false;

      OnDeadUE?.Invoke();

      SpameoReward();

      for (int i = 0; i < _observers.Count; i++)
      {
         _observers[i].OnDead();
      }
      _observers.Clear();
   }
   #endregion



   #region Coroutines
   private IEnumerator ActualizarRuta(float delay)
   {
      yield return new WaitForSeconds(delay);

      while (true)
      {
         if (_agent.enabled && _agent.isOnNavMesh && _target != null)
         {
            _agent.SetDestination(_target.position);
         }
         yield return new WaitForSeconds(0.5f);
      }
   }
   //Coroutina que ralentiza al enemigo cuando es golpeado por la municion de caca
   private IEnumerator Ralentizacion()
   {
      _agent.speed = _actualSpeed * 0.5f;
      yield return new WaitForSeconds(1f);
      _agent.speed = _actualSpeed;
      _ralentizacion = null;
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

      float delay = Random.Range(0f, 0.5f);

      StartCoroutine(ActualizarRuta(delay));

      OnInizialize?.Invoke();
   }
   public override void Deactivate()
   {
      base.Deactivate();

      StopAllCoroutines();
      _animator.enabled = false;
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























































