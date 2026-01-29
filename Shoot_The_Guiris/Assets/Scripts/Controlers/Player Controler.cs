using Unity.Mathematics;
using UnityEngine;

public class PlayerControler : CustomMonoBehaviour
{
    #region Variables
    [SerializeField] bool showGizmos = true;

    [Header("Player Movement")]
    [SerializeField] float _movementSpeed = 8f;
    [SerializeField] float _rotationSpeed = 14f;
    [SerializeField] float _acceleration = 30f;
    [SerializeField] float _deceleration = 40f;
    [SerializeField] Transform _camera;
    [SerializeField] Transform _childTransform;
    Vector3 _lastMoveDirection;

    [Header("Aiming")]
    [SerializeField] float _camRayLengt;
    [SerializeField] float _maxDistanceSide = 3f;
    [SerializeField] LayerMask _pointerLayer;
    [SerializeField] Transform _aimingPivot;
    [SerializeField] Transform _aimReference;
    //Referncia a la camara principal
    [SerializeField] Camera _mainCamera;

    [Header("Shooting")]
    [SerializeField] float _shootDeay;
    [SerializeField] float _shootTime;
    [SerializeField] Transform _shootPoint;
    [SerializeField] string _bulletType = "Wind";

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

    public override void EditorInit()
    {
        _cC = GetComponent<CharacterController>();
        _playerSound = GetComponent<AudioSource>();

    }
    #endregion
    void Start()
    {
        _mainCamera = Camera.main;
    }


    void Update()
    {
        GroundCheck();

        Controls();

        Movement();
        UpdateAnimator();
    }

    void FixedUpdate()
    {
        Aiming();
    }

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
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        if (Input.GetButtonDown("Fire1"))
        {
            Shooting();
        }
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

            // 3. Creamos la dirección final combinando los ejes
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
        if (_currentVelocity.sqrMagnitude > 0.1f)
        {
            Quaternion targetRot = Quaternion.LookRotation(_currentVelocity);
            _childTransform.rotation = Quaternion.Slerp(_childTransform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
        }
    }

    private void Aiming()
    {
        //Plano virtual a la altura del pecho para que el Raycast sea 100% estable

        Plane playerPlane = new Plane(Vector3.up, transform.position + Vector3.up * 1.5f);
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (playerPlane.Raycast(ray, out float hitDist))
        {
            Vector3 targetPoint = ray.GetPoint(hitDist);


            // Si estamos quietos, rotamos al Padre. Si nos movemos, rotamos solo al Modelo.
            Vector3 dirToMouse = targetPoint - transform.position;
            dirToMouse.y = 0;

            if (dirToMouse.sqrMagnitude > 0.1f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dirToMouse);

                if (_currentVelocity.magnitude < 0.1f)
                {
                    // Rotación de cuerpo completo en reposo
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
                }
            }


            // Usamos la posición del Padre pero CON ROTACIÓN CERO (Mundo) para que sea SIMÉTRICO

            Vector3 relativePoint = targetPoint - transform.position;

            // Convertimos el vector relativo a "Espacio Local" manualmente 
            // usando la dirección de movimiento para que el cono sea frontal
            Vector3 directionLooking = transform.forward;
            float forwardDot = Vector3.Dot(relativePoint, directionLooking);
            float rightDot = Vector3.Dot(relativePoint, transform.right);

            // Clamp simétrico puro
            float clampedX = Mathf.Clamp(rightDot, -_maxDistanceSide, _maxDistanceSide);
            float clampedZ = Mathf.Max(forwardDot, 0.5f);

            // Recomponemos la posición mundial
            Vector3 finalPos = transform.position + (transform.right * clampedX) + (transform.forward * clampedZ);
            finalPos.y = transform.position.y + 1.5f;

            _aimingPivot.position = Vector3.Lerp(_aimingPivot.position, finalPos, Time.deltaTime * 20f);
        }
    }

    private void Shooting()
    {
        if(Time.time < _shootTime) return;

        Vector3 position = Vector3.zero;
        Quaternion rotation = Quaternion.Euler(Vector3.zero);

        if(_shootPoint != null)
        {
            position = _shootPoint.position;
            rotation = Quaternion.LookRotation(_shootPoint.forward);
            _animator.SetTrigger("ShootWind");
        }
       PoolManager.Instance.Pull(_bulletType, position, rotation);
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




}
