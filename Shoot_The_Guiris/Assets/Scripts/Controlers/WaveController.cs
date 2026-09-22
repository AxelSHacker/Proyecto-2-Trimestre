
using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;
using System.Collections;
public class WaveController : MonoBehaviour, PlayerObserver
{
    public static Action OnEnemyDead;
    public static Action OnWaveIncrease;
    public Action<int> OnWaveStart;
    public Action<int> OnWaveEnd;
    [SerializeField] string[] _enemyPrefab;
    [SerializeField] string _enemyBossPrefabs;
    [SerializeField] Transform[] _spawnPoints;
    List<Transform> _spawnPoinCloseEnougt = new List<Transform>();
    [SerializeField] Transform _playerTransform;
    [SerializeField] float _distanciaMinima = 25f;
    [SerializeField] float _distanciaMaxima = 50f;
    [SerializeField] float _spawnDelay = 0.5f;
    [SerializeField] int _waveEnemyNumberMultiplier;
    [SerializeField] int _waveEnemies;
    [SerializeField] int _maxEnemiesOnScreen = 40;
    int _currentEnemiesAlive;
    [SerializeField] int _remainingEnemies;
    [SerializeField] int _remainingBosses;
    [SerializeField] TextMeshProUGUI _remainingEnemyText;
    Coroutine _actualizarDonutsCoroutine;
    float _spawnTimer;
    int _currentWave;
    void OnEnable()
    {
        BlockingWalls.OnWallDestroy += PuertaDestruida;
    }
    void OnDisable()
    {
        BlockingWalls.OnWallDestroy -= PuertaDestruida;
    }
    // Update is called once per frame
    void Update()
    {
        if (DataManager.Instance.tutorial) return;
        if (_spawnTimer <= _spawnDelay)
        {
            _spawnTimer += Time.deltaTime;
            return;
        }
        if (_waveEnemies > 0 && _currentEnemiesAlive < _maxEnemiesOnScreen)
        {
            GenerateEnemy();
            _spawnTimer = 0f;
        }
        else if (_actualizarDonutsCoroutine != null)
        {
            StopCoroutine(_actualizarDonutsCoroutine);
            _actualizarDonutsCoroutine = null;
        }

    }
    public void StartWave()
    {
        if (_actualizarDonutsCoroutine != null)
        {
            StopCoroutine(_actualizarDonutsCoroutine);
        }
        _actualizarDonutsCoroutine = StartCoroutine(ActualizarDonuts());

        _currentWave++;
        _waveEnemies = _currentWave * _waveEnemyNumberMultiplier;
        _remainingEnemies = _waveEnemies;
        _remainingEnemyText.text = "Remainings Enemies = " + _waveEnemies.ToString();
        OnWaveIncrease?.Invoke();
        OnWaveStart?.Invoke(_currentWave);
    }
    private void GenerateEnemy()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0 || _enemyPrefab == null || _enemyPrefab.Length == 0)
        {
            Debug.LogError("Spawn points or enemy prefab not set in WaveController.");
            return;
        }

        Transform puntoElegido = null;

        if (_spawnPoinCloseEnougt.Count > 0)
        {
            int randomSpawnIndex = UnityEngine.Random.Range(0, _spawnPoinCloseEnougt.Count);
            puntoElegido = _spawnPoinCloseEnougt[randomSpawnIndex];
        }
        else
        {
            int randomSpawnIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
            puntoElegido = _spawnPoints[randomSpawnIndex];
        }
        if (_remainingBosses > 0)
        {
            EnemigoIngles boss = PoolManager.Instance.Pull(_enemyBossPrefabs,
                                                          puntoElegido.position,
                                                          Quaternion.identity) as EnemigoIngles;

            boss.AddObservable(this);
            _currentEnemiesAlive++;
            _waveEnemies--;
            _remainingBosses--;
        }

        int randomPoolIndex = UnityEngine.Random.Range(0, _enemyPrefab.Length);
        EnemigoIngles enemigo = PoolManager.Instance.Pull(_enemyPrefab[randomPoolIndex],
                                                                  puntoElegido.position,
                                                                  Quaternion.identity) as EnemigoIngles;
        enemigo.AddObservable(this);
        _currentEnemiesAlive++;
        _waveEnemies--;
    }
    IEnumerator ActualizarDonuts()
    {
        while (true)
        {
            SpawnPointCloseEnougt();

            yield return new WaitForSeconds(1f);
        }
    }
    private void SpawnPointCloseEnougt()
    {
        // 1. Vaciamos la lista del "donut" para empezar el cálculo limpio en este fotograma
        _spawnPoinCloseEnougt.Clear();

        // Seguridad: Si olvidamos arrastrar al jugador en el Inspector, avisamos para evitar un crash
        if (_playerTransform == null)
        {
            Debug.LogError("No se ha asignado _playerTransform en el WaveController.");
            return; // Salimos de la función para que no rompa el juego
        }

        // Guardamos la posición actual del jugador en una variable local para acceder más rápido
        Vector3 posPlayer = _playerTransform.position;

        // OPTIMIZACIÓN MÓVIL: Elevamos los límites al cuadrado (Radio * Radio).
        // Esto nos permite comparar distancias usando 'sqrMagnitude' sin tener que calcular 
        // la pesadísima raíz cuadrada que exige la fórmula de Pitágoras tradicional.
        float minDistCuadrado = _distanciaMinima * _distanciaMinima;
        float maxDistCuadrado = _distanciaMaxima * _distanciaMaxima;

        // 2. Recorremos TODOS los puntos de spawn que hay repartidos por el mapa grande
        for (int i = 0; i < _spawnPoints.Length; i++)
        {
            // Si por error hay un hueco vacío (null) en el array del Inspector, nos lo saltamos
            if (_spawnPoints[i] == null) continue;

            // Calculamos el vector diferencia (dirección y distancia) entre el punto de spawn y el jugador
            Vector3 diferencia = _spawnPoints[i].position - posPlayer;

            // Obtenemos la longitud del vector AL CUADRADO (X*X + Y*Y + Z*Z). Ultra rápido para la CPU.
            float distanciaAlCuadrado = diferencia.sqrMagnitude;

            // 3. COMPROBACIÓN DEL DONUT:
            // ¿La distancia al cuadrado es mayor que el mínimo (625) y menor que el máximo (2500)?
            if (distanciaAlCuadrado >= minDistCuadrado && distanciaAlCuadrado <= maxDistCuadrado)
            {
                // Si cumple ambas condiciones, el punto está en la "zona dulce" y lo guardamos como válido
                _spawnPoinCloseEnougt.Add(_spawnPoints[i]);
            }
        }
    }
    private void PuertaDestruida(GameObject wall)
    {
        if (_remainingBosses >= 15) return;
        else
        _remainingBosses++;
    }
    #region PlayerObserver implementation
    public void OnHealtUpdate(float currentealt, float maxealt)
    {

    }
    public void OnHit()
    {

    }
    public void OnDead()
    {
        OnEnemyDead?.Invoke();
        _currentEnemiesAlive--;
        _remainingEnemies--;
        _remainingEnemyText.text = "Remainings Enemies = " + _remainingEnemies.ToString();
        if (_remainingEnemies <= 0)
        {
            //StartWave();
            OnWaveEnd?.Invoke(_currentWave);
        }
    }
    public void OnAtaqueEspecial(float timer, float time)
    {

    }
    public void OnDasch(float timer, float time)
    {

    }
    #endregion
}
