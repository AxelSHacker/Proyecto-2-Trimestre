
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : CustomMonoBehaviour, IDamageabe<float>, IObservable<IDamageableObserver>
{
    #region Variables
    [SerializeField] bool showGizmos = true;
    [SerializeField] PlayerInput _playerinput;

    [Header("Player Movement")]
    [SerializeField] float _movementSpeed = 8f;
    [SerializeField] float _rotationSpeed = 14f;
    [SerializeField] float _acceleration = 30f;
    [SerializeField] float _deceleration = 40f;
    [SerializeField] Transform _camera;
    [SerializeField] Transform _childTransform;


    [Header("Aiming")]
    [SerializeField] float _camRayLengt;
    [SerializeField] float _maxDistanceSide = 3f;
    [SerializeField] LayerMask _pointerLayer;
    [SerializeField] Transform _aimingPivot;
    [SerializeField] Transform _aimReference;
    //Referncia a la camara principal
    [SerializeField] Camera _mainCamera;

    [Header("Shooting")]
    [SerializeField] GameObject[] _weaponsObjects;
    [SerializeField] float _shootDelayWind;
    [SerializeField] float _shootDelayPoop;
    [SerializeField] Vector3 _targetPoint;
    [SerializeField] Transform _shootPointWind;
    [SerializeField] Transform _shootingPointPoop;
    [SerializeField] string _bulletType;
    [SerializeField] int _weaponIndex = 0;
    [SerializeField] float _fireRateWind;
    [SerializeField] float _fireRatePoop;
    [SerializeField] int _cargadorCaca;
     bool _disparando;

    [Header("Physics")]
    [SerializeField] CharacterController _cC;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] Transform _groundCheckPoint;
    [SerializeField] Vector3 _groundCheckSize;
    bool _grounded;

    [Header("Engine Sound")]
    [SerializeField] AudioSource _playerSound;
    [SerializeField] float _playerBasePitch = 0.4f;
    [SerializeField] float _playerMaxPitch = 3f;
    [Header("Effects")]
    [SerializeField] ParticleSystem[] _dustParticles;
    [SerializeField] Transform _modelTransform;
    [SerializeField] float _horizontal = 0f;
    [SerializeField] float _vertical = 0f;
    [SerializeField] Vector3 _direccion;
    [SerializeField] Vector3 _desireVelocity;
    [SerializeField] Vector3 _currentVelocity;

    [Header("Animator")]
    [SerializeField] Animator _animator;
    #region IDamagables
    [SerializeField] float _maxhealth;
    [SerializeField] float _currentealth;
    public float Maxhealt { get => _maxhealth; }
    public float Currentealt { get => _currentealth; }
    public bool IsDead => _currentealth <= 0;
    #endregion


    public override void EditorInit()
    {
        _cC = GetComponent<CharacterController>();
        _playerSound = GetComponent<AudioSource>();

    }
    #endregion
    void Start()
    {
        _shootDelayWind = 0;
        _currentealth = _maxhealth;
        _weaponsObjects[0].SetActive(true);
        _weaponsObjects[1].SetActive(false);
        _mainCamera = Camera.main;

        _animator.SetInteger("WeapoNummer", 0);
        _weaponIndex = 0;
        _weaponsObjects[0].SetActive(true);
        _weaponsObjects[1].SetActive(false);
        _bulletType = "Wind";
        _animator.SetFloat("Velocidad de disparo", _fireRateWind);
        for (int i = 0; i < _observable.Count; i++)
        {
            _observable[i].OnHealtUpdate(_currentealth, _maxhealth);
        }

    }


    void Update()
    {
        GroundCheck();

        Controls();

        Movement();
        UpdateAnimator();
        if (_shootDelayWind > 0)
        {
            _shootDelayWind -= Time.deltaTime;
        }
        if (_shootDelayPoop > 0)
        {
            _shootDelayPoop -= Time.deltaTime;
        }
    }
    void FixedUpdate()
    {
        Aiming();
        if (_disparando)
        {
            Shooting();
        }
    }

    #region ImputSystem
    public void OnMovement(InputAction.CallbackContext context)

    {
        if (context.performed)
        {
            Vector2 input = context.ReadValue<Vector2>();
            _horizontal = input.x;
            _vertical = input.y;
        }
        if (context.canceled)
        {
            _horizontal = 0f;
            _vertical = 0f;
        }
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _disparando = true;
        }else if (context.canceled)
        {
            _disparando = false;
        }
    }
    public void OnRightDasching(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DaschRight();
        }
    }
    public void OnLeftDasching(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            DaschLeft();
        }
    }
    #endregion


    void OnDrawGizmos()
    {
        if (!showGizmos) return;

        //Cambiamos el color del Gizmos
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
    #region Method
    private void Controls()
    {
        ChangeWeapon();
        // _horizontal = Input.GetAxisRaw("Horizontal");
        // _vertical = Input.GetAxisRaw("Vertical");


        // if (Input.GetButton("Fire1"))
        // {

        //     Shooting();

        // }

    }
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

        }
        else
        {
            _direccion = Vector3.zero;
        }


        //Calculamos la vlocidad deseada en base a la direccion y la velocidad
        _desireVelocity = _direccion * _movementSpeed;

        //Aceleracion desaleresacion del personaje
        float currentAceleration = (_direccion.magnitude > 0) ? _acceleration : _deceleration;
        _currentVelocity = Vector3.MoveTowards(_currentVelocity, _desireVelocity, currentAceleration * Time.deltaTime);


        //Aplicamos gravedad
        Vector3 vectorGravity = Vector3.zero;
        if (!_grounded)
        {
            vectorGravity = Vector3.down * 9.81f;
        }
        //Aplicamos el movimiento
        _cC.Move((_currentVelocity + vectorGravity) * Time.deltaTime);

        //Rotamos si estamos en movimiento
        // if (_currentVelocity.sqrMagnitude > 0.1f)
        // {
        //     Quaternion targetRot = Quaternion.LookRotation(_currentVelocity);
        //     _childTransform.rotation = Quaternion.Slerp(_childTransform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
        // }
    }

    private void DaschRight()
    {
        _cC.Move(transform.right * 400 * Time.deltaTime);
    }
    private void DaschLeft()
    {
        _cC.Move(-transform.right * 400 * Time.deltaTime);
    }

    private void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {

            _animator.SetInteger("WeapoNummer", 0);
            _weaponIndex = 0;
            _weaponsObjects[0].SetActive(true);
            _weaponsObjects[1].SetActive(false);
            _bulletType = "Wind";
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _animator.SetInteger("WeapoNummer", 1);
            _bulletType = "ProyectilCaca";
            _weaponIndex = 1;
            _weaponsObjects[0].SetActive(false);
            _weaponsObjects[1].SetActive(true);
        }
    }

    private void Aiming()
    {
        //Plano virtual a la altura del pecho para que el Raycast sea 100% estable

        Plane playerPlane = new Plane(Vector3.up, transform.position + Vector3.up * 0.5f);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (playerPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);
            _targetPoint = targetPoint;

            Vector3 dirToMouse = targetPoint - transform.position;
            dirToMouse.y = 0;
            if (dirToMouse.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToMouse);
                float currentRotSpeed = Input.GetButtonDown("Fire1") ? _rotationSpeed * 3 : _rotationSpeed;

                if (Input.GetButton("Fire1") || _currentVelocity.magnitude < 0.1f)
                {
                    // Rotación de cuerpo completo en reposo
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * currentRotSpeed);
                    _childTransform.localRotation = Quaternion.Slerp(_childTransform.localRotation,
                                                                          Quaternion.identity,
                                                                          Time.deltaTime * currentRotSpeed);
                }
                else
                {
                    Quaternion moveRoot = Quaternion.LookRotation(_currentVelocity);
                    _childTransform.rotation = Quaternion.Slerp(_childTransform.rotation, moveRoot, Time.deltaTime * currentRotSpeed);
                }
            }

            Vector3 locaTarget = transform.InverseTransformPoint(targetPoint);
            // Clamp simétrico puro
            float clampedX = Mathf.Clamp(locaTarget.x, -_maxDistanceSide, _maxDistanceSide);
            float clampedZ = Mathf.Max(locaTarget.z, 0.5f);

            // Recomponemos la posición mundial
            Vector3 locaFinalPos = new Vector3(clampedX, 0.5f, clampedZ);
            Vector3 finalPosWorld = transform.TransformPoint(locaFinalPos);
            _aimingPivot.position = Vector3.Lerp(_aimingPivot.position, finalPosWorld, Time.deltaTime * 40);
            _aimingPivot.forward = transform.forward;
        }
    }
    private void Shooting()
    {
        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(Vector3.zero);

        if (_shootPointWind != null && _bulletType == "Wind")
        {
            if (_shootDelayWind > 0) return;
            DisparodeViento(position, rotation);
        }
        else if (_shootingPointPoop != null && _bulletType == "ProyectilCaca")
        {
            if (_shootDelayPoop >  0) return;
            DisparodeCaca(position, rotation);
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
    #region  Funciones funcionales
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
        position = _shootingPointPoop.position;
        rotation = Quaternion.LookRotation(transform.forward);
        PoolManager.Instance.Pull(_bulletType, position, rotation);
    }
    #endregion
    #region IDamagable && Observable
    public void TakeDamag(float damage, Vector3 impactPoint = default(Vector3))
    {
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
    private List<IDamageableObserver> _observable;
    public void AddObservable(IDamageableObserver observable)
    {
        if (_observable == null)
        {
            _observable = new List<IDamageableObserver>();
        }
        _observable.Add(observable);

    }

    public void RemoveObservable(IDamageableObserver observable)
    {
        if (_observable == null)
        {
            _observable = new List<IDamageableObserver>();
        }
        _observable.Remove(observable);
    }


    #endregion























}
