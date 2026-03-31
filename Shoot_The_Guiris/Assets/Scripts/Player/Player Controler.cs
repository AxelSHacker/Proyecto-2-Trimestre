
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.UI;



public class PlayerControler : CustomMonoBehaviour, IDamageabe<float>, IObservable<PlayerObserver>
{
    #region Variables
    [SerializeField] bool showGizmos = true;
    [SerializeField] PlayerInput _playerinput;
    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] TextMeshProUGUI _remainingBulletsText;
    [SerializeField] TextMeshProUGUI _remainingAmmoText;
    [SerializeField] RandoSoundEffecs _randomSoundEffect;

    [Header("Scriptable Objects")]
    [SerializeField] Armas_SO _armaViento;
    [SerializeField] Armas_SO _armaCaca;
    [SerializeField] Armas_SO _armaTinta;
    Armas_SO _armaEquipada;

    [Header("Player Movement")]
    [SerializeField] float _movementSpeed = 8f;
    [SerializeField] float _normalMoveSpeed;
    [SerializeField] Transform _camera;
    [SerializeField] Transform _childTransform;


    [Header("Aiming")]
    [SerializeField] Vector2 _posicionDelRaton;
    [SerializeField] float _camRayLengt;
    [SerializeField] float _maxDistanceSide = 3f;
    [SerializeField] LayerMask _pointerLayer;
    [SerializeField] Transform _aimingPivot;
    [SerializeField] Transform _aimReference;
    //Referncia a la camara principal
    [SerializeField] Camera _mainCamera;
    Vector2 _actualMouseP;
    Vector2 _lastMouseP;

    [Header("Shooting")]
    [SerializeField] GameObject[] _weaponsObjects;
    // float _shootDelayWind;
    // float _shootDelayPoop;
    // float _shootDelayTinta;
    float _nextShootTime;
    [SerializeField] Vector3 _targetPoint;
    Transform _puntodeDisparo;
    // [SerializeField] Transform _shootPointWind;
    // [SerializeField] Transform _shootingPointPoop;
    // [SerializeField] Transform _shootingpointPulpo;
    // [SerializeField] string _bulletType;
    // [SerializeField] float _fireRateWind;
    // [SerializeField] float _fireRatePoop;
    // [SerializeField] float _fireRateTinta;
    private int _balasEnCargador;
    private int _balasEnReserva;
    Dictionary<Armas_SO, int> _municionenCargador = new Dictionary<Armas_SO, int>();
    Dictionary<Armas_SO, int> _municionenReserva = new Dictionary<Armas_SO, int>();
    Dictionary<Armas_SO, float> _tiempodeEspera = new Dictionary<Armas_SO, float>();
    // int _cargadorCaca = 20;
    // int _cargadorActualCaca;
    // [SerializeField] int _capacidadActualCaca;
    // int _maxCapacidadCaca = 200;
    // int _cargadorTinta = 1;
    // [SerializeField] int _capacidadActualTinta;
    // int _maxCapacidadTinta = 15;
    bool _disparando;
    [SerializeField] float _misiverticalOffset = -5f;
    float _multiplicadorCadencia = 1f;

    [Header("Dasching")]
    [SerializeField] float _daschTime;
    [SerializeField] float _daschTimer;
    [SerializeField] float _daschForce;

    [Header("Ataque Especial")]
    [SerializeField] float _ataqueEspecialTime;
    [SerializeField] float _ataqueEspecialTimer;

    [Header("Money")]
    [SerializeField] int money;
    public int Money => money;
    public bool comprar = false;

    [Header("Physics")]
    [SerializeField] CharacterController _cC;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckPoint;
    [SerializeField] Vector3 _groundCheckSize;
    bool _grounded;

    [Header("Engine Sound")]
    [SerializeField] AudioSource _playerSound;
    //[SerializeField] float _playerBasePitch = 0.4f;
    //[SerializeField] float _playerMaxPitch = 3f;

    [Header("Pause Menu")]
    [SerializeField] CanvasGroup _menuPausa;

    [Header("Menu de Trucos")]
    [SerializeField] CanvasGroup _menuTrucos;
    [Header("Panel Derrota")]
    [SerializeField] CanvasGroup _menuDerrota;
    float timer = 1.2f;
    [Header("Effects")]
    [SerializeField] ParticleSystem[] _dustParticles;
    [SerializeField] Transform _modelTransform;
    [SerializeField] float _horizontal = 0f;
    [SerializeField] float _vertical = 0f;
    [SerializeField] Vector3 _direccion;
    [SerializeField] Vector3 _desireVelocity;
    [SerializeField] Vector3 _currentVelocity;

    [Header("Menu de Trucos")]
    [SerializeField] bool _cargadorInfinito = false;
    [SerializeField] bool _invencible = false;
    [SerializeField] Slider _velocidadJuego;
    [SerializeField] Slider _velocidadPlayer;

    [Header("Animator")]
    [SerializeField] Animator _animator;
    #endregion





    #region Unity Methods
    public override void EditorInit()
    {
        _cC = GetComponent<CharacterController>();
        _playerSound = GetComponent<AudioSource>();
    }
    void Start()
    {
        Time.timeScale = 1f;
        // _capacidadActualCaca = _maxCapacidadCaca;
        // _cargadorActualCaca = _cargadorCaca;
        // _capacidadActualTinta = _maxCapacidadTinta;
        UpdateArma(_armaViento);
        _currentealth = _maxhealth;
        _normalMoveSpeed = _movementSpeed;

        _mainCamera = Camera.main;
        _animator.SetBool("Muerto", false);
        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnHealtUpdate(_currentealth, _maxhealth);
        }
    }
    void Update()
    {
        GroundCheck();

        Contadores();

        Movement();

        UpdateAnimator();
        //Se comprobar si el arma equipada tiene enfriamiento, y si es asi, activamos el textmeshpro para mostrar el tiempo restante
        if (_armaEquipada != null && _armaEquipada.usarEnfriamiento)
        {
            if (Time.time < _tiempodeEspera[_armaEquipada])
            {
                _tiempodeEspera[_armaEquipada] -= Time.deltaTime;
            }
        }

        UpdateAmmoText();

        Death();
    }
    void FixedUpdate()
    {
        Aiming();

        if (_disparando)
        {
            //Shooting();
            Disparar();
        }
    }
    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        //Cambiamos el color del Gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #endregion






    #region ImputSystem
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _horizontal = input.x;
            _vertical = input.y;
        }
        else if (context.canceled)
        {
            _horizontal = 0f;
            _vertical = 0f;
        }
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _movementSpeed = _movementSpeed * 2;
        }
        else if (context.canceled)
        {
            _movementSpeed = _normalMoveSpeed;
        }
    }
    public void OnMouse(InputAction.CallbackContext context)
    {
        _posicionDelRaton = context.ReadValue<Vector2>();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _disparando = true;
        }
        else if (context.canceled)
        {
            _disparando = false;
        }
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            comprar = true;
        }
        else if (context.canceled)
        {
            comprar = false;
        }
    }
    public void OnExit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_menuPausa.alpha > 0)
            {
                _menuPausa.alpha = 0;
                _menuPausa.interactable = false;
                _menuPausa.blocksRaycasts = false;
                Time.timeScale = 1f;
            }
            else
            {
                _menuPausa.alpha = 1;
                _menuPausa.interactable = true;
                _menuPausa.blocksRaycasts = true;
                Time.timeScale = 0f;
            }
        }
    }
    public void OnReload(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Recopilamos informacion para actuvar o no la recarga
            int balasActuales = _municionenCargador[_armaEquipada];
            int capacidadMaxima = _armaEquipada.capacidadCargador;
            int reservaDisponible = _municionenReserva[_armaEquipada];

            //Si el cargador no esta lleno y tenemos municion en reserva, activamos la animacion de recarga
            if (balasActuales < capacidadMaxima && reservaDisponible > 0)
            {
                _animator.SetTrigger(_armaEquipada.recargarTrigger);
            }
            
        }

    }
    public void OnSpecialAttack(InputAction.CallbackContext context)
    {
        if (context.performed && _ataqueEspecialTimer <= 0)
        {
            _animator.SetTrigger("SpecialAttack");
            _ataqueEspecialTimer = _ataqueEspecialTime;
        }
    }
    public void OnRightDasching(InputAction.CallbackContext context)
    {
        if (context.performed && _daschTimer <= 0)
        {
            DaschRight();
            _daschTimer = _daschTime;
        }
    }
    public void OnLeftDasching(InputAction.CallbackContext context)
    {
        if (context.performed && _daschTimer <= 0)
        {
            DaschLeft();
            _daschTimer = _daschTime;
        }
    }
    public void Weapon1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateArma(_armaViento);
        }
    }
    public void Weapon2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateArma(_armaCaca);
        }
    }
    public void Weapon3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateArma(_armaTinta);
        }
    }
    public void Weapon4(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Arma 4 activada, pero no implementada");
            _animator.SetInteger("WeapoNummer", 3);
        }
    }
    public void CheatMenu(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_menuTrucos.alpha > 0)
            {
                _menuTrucos.alpha = 0;
                _menuTrucos.interactable = false;
                _menuTrucos.blocksRaycasts = false;
            }
            else
            {
                _menuTrucos.alpha = 1;
                _menuTrucos.interactable = true;
                _menuTrucos.blocksRaycasts = true;
            }
        }
    }





    #endregion





    #region Method
    //Comprueba el contacto con el suelo
    private void GroundCheck()
    {
        //Solo vamos a comprobar si es mayor que 0, asi que no necesitamos mas capacida de buffer
        Collider[] colliderBuffer = new Collider[1];
        //Comprobamos si hay contacto con el suelo, lo hacemos mediant un OvrlapboxnonAlloc,
        //para no consumir mas memoria de la necesaria ya que esta funcion la vamos a hacer de manera continua
        Physics.OverlapBoxNonAlloc(_groundCheckPoint.position,
                                    _groundCheckSize / 2f,
                                    colliderBuffer,
                                    transform.rotation,
                                    _groundLayer);
        //Actualitzamos el estado de _grounded
        _grounded = colliderBuffer[0] != null;
    }
    private void Movement()
    {
        if (IsDead) return;
        //Componemos el vector d direccion deseado a partir del imput
        Vector3 inputInput = new Vector3(_horizontal, 0f, _vertical);

        if (inputInput.magnitude > 0.1f)
        {
            // 2. Extraemos las direcciones de la cámara (ignorando la inclinación vertical)
            Vector3 camForward = _camera.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = _camera.right;
            camRight.y = 0;
            camRight.Normalize();
            // 'W/S' mueve en el forward de la cámara, 'A/D' en el right de la cámara
            _direccion = (camForward * _vertical + camRight * _horizontal).normalized;

            _desireVelocity = _direccion * _movementSpeed;
        }
        else
        {
            _direccion = Vector3.zero;
            _currentVelocity = Vector3.zero;
        }
        //Calculamos la vlocidad deseada en base a la direccion y la velocidad
        _currentVelocity = _direccion * _movementSpeed;
        //Aplicamos gravedad
        Vector3 vectorGravity = Vector3.zero;
        if (!_grounded)
        {
            vectorGravity = Vector3.down * 9.81f;
        }
        //Aplicamos el movimiento
        _cC.Move((_currentVelocity + vectorGravity) * Time.deltaTime);

    }
    private void DaschRight()
    {
        _animator.SetTrigger("EsquivarRight");
    }
    private void DaschLeft()
    {
        _animator.SetTrigger("EsquivarLeft");
    }
    private void Aiming()
    {
        if (IsDead) return;

        // 1. OBTENER EL PUNTO DEL MUNDO (Para la mira visual)
        Plane playerPlane = new Plane(Vector3.up, new Vector3(0, transform.position.y, 0));
        Ray ray = _mainCamera.ScreenPointToRay(_posicionDelRaton);

        if (playerPlane.Raycast(ray, out float hitDist))
        {
            _targetPoint = ray.GetPoint(hitDist);
        }

        // 2. ROTACIÓN DEL CUERPO (Lógica de "Espaldas")
        // Calculamos la dirección hacia el punto
        Vector3 dirToMouse = _targetPoint - transform.position;
        dirToMouse.y = 0;

        if (dirToMouse.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dirToMouse);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime);
        }
        // Convierte el punto del ratón de coordenadas del mundo a coordenadas locales del personaje.
        Vector3 locaTarget = transform.InverseTransformPoint(_targetPoint);
        // Limita qué tan a la izquierda o derecha puede ir el punto de mira.
        float clampedX = Mathf.Clamp(locaTarget.x, -_maxDistanceSide, _maxDistanceSide);
        // Asegura que el punto de mira esté siempre al menos a 0.5 unidades frente al personaje.
        float clampedZ = Mathf.Max(locaTarget.z, 0.5f);

        // Convierte esa posición limitada de vuelta a coordenadas del mundo.
        Vector3 finalPosWorld = transform.TransformPoint(new Vector3(clampedX, 0.5f, clampedZ));

        // Mueve el objeto "_aimingPivot" (donde apunta el arma) a esa posición con un suavizado muy rápido (* 40).
        _aimingPivot.position = Vector3.Lerp(_aimingPivot.position, finalPosWorld, Time.deltaTime * 20);
        // Asegura que el pivote siempre mire hacia adelante respecto al personaje.
        _aimingPivot.forward = transform.forward;



    }
    private void Death()
    {
        if (IsDead)
        {
            timer -= Time.deltaTime;
            _animator.SetBool("Muerto", true);
            if (timer <= 0f)
            {
                Time.timeScale = 0;
                _menuDerrota.alpha = 1;
                _menuDerrota.interactable = true;
                _menuDerrota.blocksRaycasts = true;
            }
        }
    }
    // private void Shooting()
    // {
    //     //Reiniciamos las posiciones para que no se sumen o de errores
    //     Vector3 position = Vector3.zero;
    //     Quaternion rotation = Quaternion.Euler(Vector3.zero);
    //     //Comprobamos queel proyectil de viento esta activo para poder dispararlo y los demas no
    //     if (_shootPointWind != null && _bulletType == "Wind")
    //     {
    //         if (_shootDelayWind > 0) return;
    //         DisparodeViento(position, rotation);
    //     }
    //     //Comprobamos queel proyectil de caca esta activo para poder dispararlo y los demas no
    //     else if (_shootingPointPoop != null && _bulletType == "ProyectilCaca" && _capacidadActualCaca > 0)
    //     {
    //         if (_shootDelayPoop > 0 || _cargadorActualCaca <= 0) return;
    //         DisparodeCaca(position, rotation);
    //         if (_cargadorInfinito) return;
    //         _cargadorActualCaca--;
    //     }
    //     //Comprobamos queel proyectil del pulpo esta activo para poder dispararlo y los demas no
    //     else if (_shootingpointPulpo != null && _bulletType == "Misil" && _capacidadActualTinta > 0)
    //     {
    //         if (_cargadorTinta <= 0 || _shootDelayTinta > 0) return;
    //         DisparodeTinta(position, rotation);
    //         if (_cargadorInfinito) return;
    //         _cargadorTinta--;
    //     }
    // }
    // private void ParticleFX()
    // {
    //     //Si estamos en el suelo y nos movemos, activamos las partículas de polvo
    //     if (_grounded && _currentVelocity.magnitude > 0.1f)
    //     {
    //         foreach (ParticleSystem ps in _dustParticles)
    //         {
    //             if (!ps.isPlaying)
    //                 ps.Play();
    //         }
    //     }
    //     else
    //     {
    //         foreach (ParticleSystem ps in _dustParticles)
    //         {
    //             ps.Stop();
    //         }
    //     }
    // }
    private void UpdateAnimator()
    {
        _animator.SetFloat("Velocity", _currentVelocity.magnitude, 0.001f, Time.deltaTime);
    }
    private void UpdateArma(Armas_SO armas)
    {

        //Actiuvams el modelo visual del arma
        for (int i = 0; i < _weaponsObjects.Length; i++)
        {
            bool activar = (i == armas.modelIndex);
            _weaponsObjects[i].SetActive(activar);
            //Buscamos el punto de disparo dentro del modelo del arma
            if (activar)
            {
                //Buscamos el punto del arma segun el nombre que hayamos puesto en el Scriptable Object
                _puntodeDisparo = _weaponsObjects[i].transform.Find(armas.nombreShootPoint);
                //Si por casualidad no pilla ninguno , el primer hijo es el elegido
                if (_puntodeDisparo == null) _puntodeDisparo = _weaponsObjects[i].transform.GetChild(0);
            }
        }
        //Asignamos el ID del arma al animator para que cambie a la animacion correspondiente
        _animator.SetInteger("WeapoNummer", armas.animatorID);
        //Reseteamos el tiempo de espera del arma equipada, para que al cambiar de arma no haya que esperar a que se recargue o algo similar
        if (!_tiempodeEspera.ContainsKey(armas))
        {
            _tiempodeEspera[armas] = 0f;
        }
        //Si es la primera vez que equipamos el arma, inicializamos las balas en cargador y reserva con los valores del Scriptable Object
        if (!_municionenCargador.ContainsKey(armas))
        {
            _municionenCargador[armas] = armas.capacidadCargador;
            _municionenReserva[armas] = armas.municionTotal;
        }
        //Asignamos el arma equipada a la que hemos elegido
        _armaEquipada = armas;
        UpdateAmmoText();
    }
    private void Disparar()
    {
        if (_armaEquipada == null || Time.time < _tiempodeEspera[_armaEquipada]) return;
        //Si no usa enfriamiento pasamos de recargar
        if (!_armaEquipada.usarEnfriamiento)
        {
            //Controlamos la cadencia de disparo
            if (_municionenCargador[_armaEquipada] <= 0)
            {
                _animator.SetTrigger(_armaEquipada.recargarTrigger);
                return;
            }
        }

        //Usamos el canion que buscamos con anterioridad
        Vector3 pos = _puntodeDisparo.position;
        Quaternion rot = Quaternion.LookRotation(_puntodeDisparo.forward);

        //Ajustamos la velocidad de la animacion segun la cadencia de disparo del arma equipada y el multiplicador de cadencia
        float velocidadAnimacion = 1f / (_armaEquipada.fireRate * _multiplicadorCadencia);
        _animator.SetFloat("Velocidad de disparo", velocidadAnimacion);
        //Ajustamos el trigger del animator para que dispare la animacion correspondiente al arma
        _animator.SetTrigger(_armaEquipada.shootTrigger);

        if (_armaEquipada.esMisil)
        {
            //Disparamos el proyectil correspondiente al arma equipada, usando el sistema de pooling
            Misil tempMisil = PoolManager.Instance.Pull(_armaEquipada.bulletPoolID, pos, rot) as Misil;
            Vector3 puntodeImpacto = _aimingPivot.position;
            puntodeImpacto.y += _misiverticalOffset;
            tempMisil.IniciarMisil(pos, puntodeImpacto, transform.position);
        }
        else
        {
            //Disparamos el proyectil correspondiente al arma equipada, usando el sistema de pooling
            PoolManager.Instance.Pull(_armaEquipada.bulletPoolID, pos, rot);
        }

        //Actualizamos el tiempo del siguiente disparo segun la cadencia de disparo del arma equipada y el multiplicador de cadencia
        float collDown = _armaEquipada.fireRate * _multiplicadorCadencia;
        _tiempodeEspera[_armaEquipada] = Time.time + collDown;
        //Si el cargador infinito no esta activado y el arma no usa enfriamiento, restamos una bala al cargador
        if (!_cargadorInfinito && !_armaEquipada.usarEnfriamiento)
        {
            _municionenCargador[_armaEquipada]--;
        }
        UpdateAmmoText();
    }









    #endregion




    ///<summary>
    #region Funciones funcionales
    //Estas funciones se encargan de disparar cada tipo de proyectil, ahora mismo estan un poco repetitivas, pero las dejo asi por si quiero hacer algo especifico para cada una, como efectos o sonidos diferentes
    // private void DisparodeViento(Vector3 position, Quaternion rotation)
    // {
    //     _animator.SetTrigger("ShootWind");
    //     _shootDelayWind = _fireRateWind;
    //     position = _shootPointWind.position;
    //     rotation = Quaternion.LookRotation(transform.forward);
    //     PoolManager.Instance.Pull(_bulletType, position, rotation);
    // }
    // private void DisparodeCaca(Vector3 position, Quaternion rotation)
    // {
    //     _animator.SetTrigger("ShootPoop");
    //     _shootDelayPoop = _fireRatePoop;
    //     _animator.SetFloat("Velocidad de disparo", 1f / _fireRatePoop);
    //     position = _shootingPointPoop.position;
    //     rotation = Quaternion.LookRotation(transform.forward);
    //     PoolManager.Instance.Pull(_bulletType, position, rotation);
    // }
    // private void DisparodeTinta(Vector3 position, Quaternion rotacion)
    // {
    //     _animator.SetTrigger("ShootTinta");
    //     _shootDelayTinta = _fireRateTinta;
    //     position = _shootingpointPulpo.position;
    //     rotacion = Quaternion.LookRotation(transform.forward);
    //     Misil tempMisil = PoolManager.Instance.Pull(_bulletType, position, rotacion) as Misil;
    //     Vector3 puntodeImpacto = _aimingPivot.position;
    //     puntodeImpacto.y += _misiverticalOffset;
    //     tempMisil.IniciarMisil(position, puntodeImpacto, transform.position);
    // }

    //Antiguas Funciones para cambiar de arma, ahora se hace mediante el Scriptable Object y la funcion UpdateArma, pero las dejo comentadas por si acaso
    // 
    // private void ArmadeViento()
    // {
    //     _weaponsObjects[1].SetActive(false);
    //     _weaponsObjects[2].SetActive(false);
    //     _weaponsObjects[0].SetActive(true);

    //     _animator.SetInteger("WeapoNummer", 0);

    //     var derecha = _rightConstrain.data;
    //     var izquierda = _leftConstrain.data;

    //     derecha.target = _rightHandPosition[0];
    //     izquierda.target = _leftHandPosition[0];

    //     _rightConstrain.data = derecha;
    //     _leftConstrain.data = izquierda;

    //     _bulletType = "Wind";
    // }
    // private void ArmaDeCaca()
    // {
    //     _weaponsObjects[2].SetActive(false);
    //     _weaponsObjects[0].SetActive(false);
    //     _weaponsObjects[1].SetActive(true);

    //     _animator.SetInteger("WeapoNummer", 1);

    //     var derecha = _rightConstrain.data;
    //     var izquierda = _leftConstrain.data;

    //     derecha.target = _rightHandPosition[1];
    //     izquierda.target = _leftHandPosition[1];

    //     _rightConstrain.data = derecha;
    //     _leftConstrain.data = izquierda;

    //     _bulletType = "ProyectilCaca";
    // }
    // private void ArmaDeTinta()
    // {
    //     _weaponsObjects[0].SetActive(false);
    //     _weaponsObjects[1].SetActive(false);
    //     _weaponsObjects[2].SetActive(true);

    //     _animator.SetInteger("WeapoNummer", 2);

    //     var derecha = _rightConstrain.data;
    //     var izquierda = _leftConstrain.data;

    //     derecha.target = _rightHandPosition[2];
    //     izquierda.target = _leftHandPosition[2];

    //     _rightConstrain.data = derecha;
    //     _leftConstrain.data = izquierda;

    //     _bulletType = "Misil";
    // }
    /// </summary>


    private void Contadores()
    {
        // if (_shootDelayWind > 0)
        // {
        //     _shootDelayWind -= Time.deltaTime;
        // }
        // if (_shootDelayPoop > 0f)
        // {
        //     _shootDelayPoop -= Time.deltaTime;
        // }
        // if (_shootDelayTinta > 0f)
        // {
        //     _shootDelayTinta -= Time.deltaTime;
        // }
        if (_ataqueEspecialTimer > 0)
        {
            _ataqueEspecialTimer -= Time.deltaTime;
            for (int i = 0; i < _observable.Count; i++)
            {
                _observable[i].OnAtaqueEspecial(_ataqueEspecialTimer, _ataqueEspecialTime);
            }
        }
        if (_daschTimer > 0)
        {
            _daschTimer -= Time.deltaTime;
            for (int i = 0; i < _observable.Count; i++)
            {
                _observable[i].OnDasch(_daschTimer, _daschTime);
            }
        }
    }
    private void UpdateAmmoText()
    {
        if (_armaEquipada == null) return;

        if (_armaEquipada.usarEnfriamiento)
        {
            //Actualizamos el texto de la UI para mostrar el tiempo restante para el siguiente disparo
            float tiempoRestante = _tiempodeEspera[_armaEquipada] - Time.time;
            _remainingBulletsText.text = tiempoRestante > 0 ? tiempoRestante.ToString("F1") + "s Espera" : "Listo";
            _remainingAmmoText.text = "";
        }
        else
        {
            //Obtenemos los datos del Diccionario
            int balasEnCargador = _municionenCargador[_armaEquipada];
            int balasEnReserva = _municionenReserva[_armaEquipada];
            //Actualizamos el texto de la UI
            _remainingBulletsText.text = balasEnCargador.ToString();
            _remainingAmmoText.text = balasEnReserva.ToString();
        }
    }
    public void RecibirVida(float cantidad)
    {
        _currentealth += cantidad;
        _currentealth = Mathf.Clamp(_currentealth, 0, _maxhealth);

        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnHealtUpdate(_currentealth, _maxhealth);
        }
    }
    public void RecibirDinero()
    {
        money += Random.Range(300, 500);
        _moneyText.text = money.ToString() + " €";
    }
    public void QuitarDinero(int cantidad)
    {
        money -= cantidad;
        _moneyText.text = money.ToString();
    }
    public void RecibirMunicion(int cantidad)
    {
        List<Armas_SO> armas = new List<Armas_SO>(_municionenReserva.Keys);
        //Recorremos todas las armas en el diccionario y les añadimos la cantidad de municion recibida
        foreach (Armas_SO arma in armas)
        {
            _municionenReserva[arma] += cantidad;

            _municionenReserva[arma] = Mathf.Clamp(_municionenReserva[arma], 0, arma.municionTotal);
        }
        UpdateAmmoText();
    }
    public void ContinueButton()
    {
        _menuPausa.alpha = 0;
        _menuPausa.interactable = false;
        _menuPausa.blocksRaycasts = false;
        Time.timeScale = 1f;
    }


    #endregion






    #region Menu de Trucos
    public void ControlTiempoJuego(float nuevoValor)
    {
        Time.timeScale = nuevoValor;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        _velocidadJuego.value = nuevoValor;
    }
    public void CargadorInfinito()
    {
        _cargadorInfinito = true;
    }
    public void Mas10000()
    {
        money += 10000;
    }
    public void DisparoVientoConstante()
    {
        _multiplicadorCadencia = (_multiplicadorCadencia == 1f) ? 0.5f : 1f;
    }
    public void AtaqueEspecialDashDElayReducido()
    {
        _daschTime = 1f;
        _ataqueEspecialTime = 1f;
    }
    public void PlayerSpeedModificador(float modificador)
    {
        _movementSpeed = modificador;
        _velocidadPlayer.value = modificador;
    }
    public void Invencivilidad()
    {
        _invencible = true;
    }
    public void ReseteoGeneral()
    {
        Time.timeScale = 1f;
        _cargadorInfinito = false;
        _movementSpeed = 30f;
        _invencible = false;
        _multiplicadorCadencia = 1f;
    }
    #endregion






    #region Animation Events
    public void EvetoDisparo()
    {
        if (_armaEquipada == null) return;
        //Obtenemos los datos del Scriptable Object
        int maxEnCargador = _armaEquipada.capacidadCargador;

        //Obtenemos cuantas balas quedan en el cargador del arma equipada
        int balasEnCargador = _municionenCargador[_armaEquipada];
        int balasEnReserva = _municionenReserva[_armaEquipada];

        //Logica del calculo
        int balasFaltantes = maxEnCargador - balasEnCargador;
        int balasARecargar = Mathf.Min(balasFaltantes, balasEnReserva);

        //Actualizamos las balas en cargador y reserva
        _municionenCargador[_armaEquipada] += balasARecargar;
        _municionenReserva[_armaEquipada] -= balasARecargar;

        //Reseteamos los triggers de recarga para evitar que se queden pillados
        _animator.ResetTrigger("ReloadRifle");
        _animator.ResetTrigger("ReloadBazooca");

        UpdateAmmoText();
    }
    // public void RecargacacaRealizada()
    // {
    //     int _balasFaltantes = _cargadorCaca - _cargadorActualCaca;
    //     int _balasaRecargar = Mathf.Min(_balasFaltantes, _maxCapacidadCaca);

    //     _cargadorActualCaca += _balasaRecargar;
    //     _capacidadActualCaca -= _balasaRecargar;
    //     _animator.ResetTrigger("ReloadRifle");
    // }
    // public void RecargaPulpoRealizada()
    // {
    //     _cargadorTinta++;
    //     _capacidadActualTinta--;
    //     _animator.ResetTrigger("ReloadBazooca");
    // }
    #endregion





    #region IDamagable && Observable
    [SerializeField] float _maxhealth;
    [SerializeField] float _currentealth;
    public float Maxhealt { get => _maxhealth; }
    public float Currentealt { get => _currentealth; }
    public bool IsDead => _currentealth <= 0;
    public void TakeDamag(float damage, Vector3 impactPoint = default(Vector3))
    {
        if (_invencible) return;

        _currentealth -= damage;
        _currentealth = Mathf.Clamp(_currentealth, 0, _maxhealth);
        _randomSoundEffect.PlayRandomSoundEffect();

        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnHealtUpdate(_currentealth, _maxhealth);
            _observable[i].OnHit();
        }
    }


    private List<PlayerObserver> _observable;
    public void AddObservable(PlayerObserver observable)
    {
        if (_observable == null)
        {
            _observable = new List<PlayerObserver>();
        }
        _observable.Add(observable);

    }
    public void RemoveObservable(PlayerObserver observable)
    {
        if (_observable == null)
        {
            _observable = new List<PlayerObserver>();
        }
        _observable.Remove(observable);
    }
    #endregion
}















































