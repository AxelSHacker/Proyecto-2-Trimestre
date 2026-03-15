
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControler : CustomMonoBehaviour, IDamageabe<float>, IObservable<PlayerObserver>
{
    #region Variables
    [SerializeField] bool showGizmos = true;
    [SerializeField] PlayerInput _playerinput;
    [SerializeField] TextMeshProUGUI _moneyText;
    [SerializeField] TextMeshProUGUI _remainingBulletsText;
    [SerializeField] TextMeshProUGUI _remainingAmmoText;

    [Header("Player Movement")]
    [SerializeField] float _movementSpeed = 8f;
    [SerializeField] float _normalMoveSpeed;
    [SerializeField] float _rotationSpeed = 14f;
    [SerializeField] float _acceleration = 30f;
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
    [SerializeField] TwoBoneIKConstraint _rightConstrain;
    [SerializeField] TwoBoneIKConstraint _leftConstrain;
    [SerializeField] Transform[] _rightHandPosition;
    [SerializeField] Transform[] _leftHandPosition;
    Vector3 _ultimaPosicionRaton;
    bool _movimientoRaton;
    [SerializeField] float _sensibilidadDeteccion = 0.1f;

    [Header("Shooting")]
    [SerializeField] GameObject[] _weaponsObjects;
    float _shootDelayWind;
    float _shootDelayPoop;
    float _shootDelayTinta;
    [SerializeField] Vector3 _targetPoint;
    [SerializeField] Transform _shootPointWind;
    [SerializeField] Transform _shootingPointPoop;
    [SerializeField] Transform _shootingpointPulpo;
    [SerializeField] string _bulletType;
    [SerializeField] float _fireRateWind;
    [SerializeField] float _fireRatePoop;
    [SerializeField] float _fireRateTinta;
    int _cargadorCaca = 20;
    int _cargadorActualCaca;
    [SerializeField] int _capacidadActualCaca;
    int _maxCapacidadCaca = 140;
    int _cargadorTinta = 1;
    [SerializeField] int _capacidadActualTinta;
    int _maxCapacidadTinta = 15;
    bool _disparando;
    [SerializeField] float _misiverticalOffset = -5f;

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
        _moneyText.text = money.ToString() + " €";
        Time.timeScale = 1f;
        _capacidadActualCaca = _maxCapacidadCaca;
        _cargadorActualCaca = _cargadorCaca;
        _capacidadActualTinta = _maxCapacidadTinta;
        ArmadeViento();
        _currentealth = _maxhealth;
        _normalMoveSpeed = _movementSpeed;

        _mainCamera = Camera.main;

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

        UpdateAmmoText();
    }
    void FixedUpdate()
    {
        Aiming();

        if (_disparando)
        {
            Shooting();
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
        if (context.performed && _bulletType == "ProyectilCaca" && _cargadorActualCaca < _capacidadActualCaca)
        {
            _animator.SetTrigger("ReloadRifle");
        }
        else if (context.performed && _bulletType == "Misil" && _cargadorTinta <= _capacidadActualTinta)
        {
            _animator.SetTrigger("ReloadBazooca");
        }
        else
        {
            _animator.ResetTrigger("ReloadRifle");
            _animator.ResetTrigger("ReloadBazooca");
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
            ArmadeViento();
        }
    }
    public void Weapon2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ArmaDeCaca();
        }
    }
    public void Weapon3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            ArmaDeTinta();
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
            _currentVelocity = Vector3.MoveTowards(_currentVelocity, _desireVelocity, _acceleration * Time.deltaTime);
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
        //Float para calcular si el raton esta en movimiento
        float distanciaMovimientoRaton = Vector3.Distance(_posicionDelRaton, _ultimaPosicionRaton);
        //Si el raton se esta moviendo la rotacion de la camara se permite
        if (distanciaMovimientoRaton > _sensibilidadDeteccion)
        {
            _movimientoRaton = true;
            _ultimaPosicionRaton = _posicionDelRaton;
        }
        else
        {
            _movimientoRaton = false;
        }

        //Comprobamos si el raton se esta moviendo o si la estamos disparando para rotar la camara
        if (_movimientoRaton || _disparando)
        {
            // Crea un plano matemático invisible orientado hacia arriba, a 0.5 unidades por encima del personaje.
            Plane playerPlane = new Plane(Vector3.up, transform.position + Vector3.up * 0.5f);
            // Lanza un rayo desde la cámara que pasa por la posición actual del cursor en la pantalla.
            Ray ray = _mainCamera.ScreenPointToRay(_posicionDelRaton);

            // Calcula si el rayo del ratón choca con el plano virtual creado antes.
            if (playerPlane.Raycast(ray, out float hitDist))
            {
                // Obtiene las coordenadas 3D exactas donde el rayo tocó el plano.
                Vector3 targetPoint = ray.GetPoint(hitDist);
                // Guarda ese punto en una variable global.
                _targetPoint = targetPoint;
                // Calcula la dirección desde el personaje hacia ese punto del ratón.
                Vector3 dirToMouse = targetPoint - transform.position;
                // Ignora la diferencia de altura para que el personaje no se incline.
                dirToMouse.y = 0;

                // Si el ratón no está justo encima del personaje (evita errores de rotación).
                if (dirToMouse.sqrMagnitude > 0.1f)
                {
                    // Crea la rotación necesaria para mirar hacia el ratón.
                    Quaternion targetRot = Quaternion.LookRotation(dirToMouse);
                    // Si está disparando, la rotación es 3 veces más rápida para mayor precisión.
                    float currentRotSpeed = _disparando ? _rotationSpeed * 3 : _rotationSpeed;

                    // Si está disparando o está quieto:
                    if (_disparando)
                    {
                        // Gira el cuerpo entero (el objeto principal) hacia el ratón suavemente.
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * currentRotSpeed);
                        // Resetea la rotación del "hijo" (el modelo visual) para que mire al frente.
                        _childTransform.localRotation = Quaternion.Slerp(_childTransform.localRotation,
                                                                              Quaternion.identity,
                                                                              Time.deltaTime * currentRotSpeed);
                    }
                    else if (_currentVelocity.sqrMagnitude < 0.1f)
                    {
                        // Si se está moviendo, el cuerpo principal y el modelo visual ambos miran hacia el ratón.
                        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * currentRotSpeed);
                        // _childTransform.localRotation = Quaternion.Slerp(_childTransform.localRotation,
                        //                                                       Quaternion.identity,
                        //                                                       Time.deltaTime * currentRotSpeed);
                    }
                    // Si se está moviendo pero NO disparando:
                    else
                    {
                        // El cuerpo principal sigue mirando al ratón, pero el modelo visual mira hacia la dirección del movimiento.
                        Quaternion moveRoot = Quaternion.LookRotation(_currentVelocity);
                        _childTransform.rotation = Quaternion.Slerp(_childTransform.rotation, moveRoot, Time.deltaTime * currentRotSpeed);
                    }
                }
                // Convierte el punto del ratón de coordenadas del mundo a coordenadas locales del personaje.
                Vector3 locaTarget = transform.InverseTransformPoint(targetPoint);
                // Limita qué tan a la izquierda o derecha puede ir el punto de mira.
                float clampedX = Mathf.Clamp(locaTarget.x, -_maxDistanceSide, _maxDistanceSide);
                // Asegura que el punto de mira esté siempre al menos a 0.5 unidades frente al personaje.
                float clampedZ = Mathf.Max(locaTarget.z, 0.5f);

                // Reconstruye la posición local limitada.
                Vector3 locaFinalPos = new Vector3(clampedX, 0.5f, clampedZ);
                // Convierte esa posición limitada de vuelta a coordenadas del mundo.
                Vector3 finalPosWorld = transform.TransformPoint(locaFinalPos);

                // Mueve el objeto "_aimingPivot" (donde apunta el arma) a esa posición con un suavizado muy rápido (* 40).
                _aimingPivot.position = Vector3.Lerp(_aimingPivot.position, finalPosWorld, Time.deltaTime * 40);
                // Asegura que el pivote siempre mire hacia adelante respecto al personaje.
                _aimingPivot.forward = transform.forward;
            }
        }
    }


    private void Shooting()
    {
        //Reiniciamos las posiciones para que no se sumen o de errores
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(Vector3.zero);
        //Comprobamos queel proyectil de viento esta activo para poder dispararlo y los demas no
        if (_shootPointWind != null && _bulletType == "Wind")
        {
            if (_shootDelayWind > 0) return;
            DisparodeViento(position, rotation);
        }
        //Comprobamos queel proyectil de caca esta activo para poder dispararlo y los demas no
        else if (_shootingPointPoop != null && _bulletType == "ProyectilCaca" && _capacidadActualCaca > 0)
        {
            if (_shootDelayPoop > 0 || _cargadorActualCaca <= 0) return;
            DisparodeCaca(position, rotation);
            if (_cargadorInfinito) return;
            _cargadorActualCaca--;
        }
        //Comprobamos queel proyectil del pulpo esta activo para poder dispararlo y los demas no
        else if (_shootingpointPulpo != null && _bulletType == "Misil" && _capacidadActualTinta > 0)
        {
            if (_cargadorTinta <= 0 || _shootDelayTinta > 0) return;
            DisparodeTinta(position, rotation);
            if (_cargadorInfinito) return;
            _cargadorTinta--;
        }
    }
    private void ParticleFX()
    {
        //Si estamos en el suelo y nos movemos, activamos las partículas de polvo
        if (_grounded && _currentVelocity.magnitude > 0.1f)
        {
            foreach (ParticleSystem ps in _dustParticles)
            {
                if (!ps.isPlaying)
                    ps.Play();
            }
        }
        else
        {
            foreach (ParticleSystem ps in _dustParticles)
            {
                ps.Stop();
            }
        }
    }
    private void UpdateAnimator()
    {
        _animator.SetFloat("Velocity", _currentVelocity.magnitude, 0.001f, Time.deltaTime);
    }
    #endregion





    #region Funciones funcionales
    private void DisparodeViento(Vector3 position, Quaternion rotation)
    {
        _animator.SetTrigger("ShootWind");
        _shootDelayWind = _fireRateWind;
        position = _shootPointWind.position;
        rotation = Quaternion.LookRotation(transform.forward);
        PoolManager.Instance.Pull(_bulletType, position, rotation);
    }
    private void DisparodeCaca(Vector3 position, Quaternion rotation)
    {
        _animator.SetTrigger("ShootPoop");
        _shootDelayPoop = _fireRatePoop;
        _animator.SetFloat("Velocidad de disparo", 1f / _fireRatePoop);
        position = _shootingPointPoop.position;
        rotation = Quaternion.LookRotation(transform.forward);
        PoolManager.Instance.Pull(_bulletType, position, rotation);
    }
    private void DisparodeTinta(Vector3 position, Quaternion rotacion)
    {
        _animator.SetTrigger("ShootTinta");
        _shootDelayTinta = _fireRateTinta;
        position = _shootingpointPulpo.position;
        rotacion = Quaternion.LookRotation(transform.forward);
        Misil tempMisil = PoolManager.Instance.Pull(_bulletType, position, rotacion) as Misil;
        Vector3 puntodeImpacto = _aimingPivot.position;
        puntodeImpacto.y += _misiverticalOffset;
        tempMisil.IniciarMisil(position, puntodeImpacto, transform.position);
    }
    private void ArmadeViento()
    {
        _weaponsObjects[1].SetActive(false);
        _weaponsObjects[2].SetActive(false);
        _weaponsObjects[0].SetActive(true);

        _animator.SetInteger("WeapoNummer", 0);

        var derecha = _rightConstrain.data;
        var izquierda = _leftConstrain.data;

        derecha.target = _rightHandPosition[0];
        izquierda.target = _leftHandPosition[0];

        _rightConstrain.data = derecha;
        _leftConstrain.data = izquierda;

        _bulletType = "Wind";
    }
    private void ArmaDeCaca()
    {
        _weaponsObjects[2].SetActive(false);
        _weaponsObjects[0].SetActive(false);
        _weaponsObjects[1].SetActive(true);

        _animator.SetInteger("WeapoNummer", 1);

        var derecha = _rightConstrain.data;
        var izquierda = _leftConstrain.data;

        derecha.target = _rightHandPosition[1];
        izquierda.target = _leftHandPosition[1];

        _rightConstrain.data = derecha;
        _leftConstrain.data = izquierda;

        _bulletType = "ProyectilCaca";
    }
    private void ArmaDeTinta()
    {
        _weaponsObjects[0].SetActive(false);
        _weaponsObjects[1].SetActive(false);
        _weaponsObjects[2].SetActive(true);

        _animator.SetInteger("WeapoNummer", 2);

        var derecha = _rightConstrain.data;
        var izquierda = _leftConstrain.data;

        derecha.target = _rightHandPosition[2];
        izquierda.target = _leftHandPosition[2];

        _rightConstrain.data = derecha;
        _leftConstrain.data = izquierda;

        _bulletType = "Misil";
    }
    private void Contadores()
    {
        if (_shootDelayWind > 0)
        {
            _shootDelayWind -= Time.deltaTime;
        }
        if (_shootDelayPoop > 0f)
        {
            _shootDelayPoop -= Time.deltaTime;
        }
        if (_shootDelayTinta > 0f)
        {
            _shootDelayTinta -= Time.deltaTime;
        }
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
        if (_bulletType == "Wind")
        {
            _remainingBulletsText.text = _shootDelayWind.ToString("F2") + "Wait";
        }
        else if (_bulletType == "ProyectilCaca")
        {
            _remainingBulletsText.text = _cargadorActualCaca.ToString() + " Magazine";
            _remainingAmmoText.text = _capacidadActualCaca.ToString() + " Ammo";
        }
        else if (_bulletType == "Misil")
        {
            _remainingBulletsText.text = _cargadorTinta.ToString() + " Magazine";
            _remainingAmmoText.text = _capacidadActualTinta.ToString() + " Ammo";
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
    public void RecibirDinero(int cantidad)
    {
        money += cantidad;
        _moneyText.text = money.ToString() + " €";
    }
    public void QuitarDinero(int cantidad)
    {
        money -= cantidad;
        _moneyText.text = money.ToString();
    }
    public void RecibirMunicion(int cantidad, string tipoMunicion)
    {
        if (tipoMunicion == "ProyectilCaca")
        {

            _capacidadActualCaca += cantidad;
            _capacidadActualCaca = Mathf.Clamp(_capacidadActualCaca, 0, _maxCapacidadCaca);
        }
        else if (tipoMunicion == "Misil")
        {

            _capacidadActualTinta += cantidad;
            _capacidadActualTinta = Mathf.Clamp(_capacidadActualCaca, 0, _maxCapacidadTinta);
        }

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
    public void DisparoVientoConstante()
    {
        _fireRateWind = 1f;
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
    public void ReseteoGeneral()
    {
        Time.timeScale = 1f;
        _cargadorInfinito = false;
        _fireRateWind = 10f;
        _movementSpeed = 30f;
        _invencible = false;
    }
    public void Invencivilidad()
    {
        _invencible = true;
    }
    #endregion






    #region Animation Events

    public void RecargacacaRealizada()
    {
        _cargadorActualCaca += _cargadorCaca;
        _capacidadActualCaca -= _cargadorCaca;
    }



    public void RecargaPulpoRealizada()
    {
        _cargadorTinta++;
        _capacidadActualTinta--;
    }
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

        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnHealtUpdate(_currentealth, _maxhealth);
            _observable[i].OnHit();
        }
        if (IsDead)
        {
            Death();
        }
    }
    private void Death()
    {
        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnDead();
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































