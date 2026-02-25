using Unity.VisualScripting;
using UnityEngine;
using System;
public class WaveController : MonoBehaviour, PlayerObserver
{
    public Action<int> OnWaveStart;
    public Action<int> OnWaveEnd;
    [SerializeField] string[] _enemyPrefab;
    [SerializeField] Transform[] _spawnPoints;
    [SerializeField] float _spawnDelay  = 0.5f;
    [SerializeField] int _waveEnemyNumberMultiplier;
    [SerializeField] int _waveEnemies;
    [SerializeField] int _remainingEnemies;
    float _spawnTimer;
    int _currentWave;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_spawnTimer <= _spawnDelay)
        {
            _spawnTimer += Time.deltaTime;
            return;
        }
        if (_waveEnemies > 0)
        {
            GenerateEnemy();
            _spawnTimer = 0f;
        }
    }
    public void StartWave()
    {
        _currentWave++;
        _waveEnemies = _currentWave * _waveEnemyNumberMultiplier;
        _remainingEnemies = _waveEnemies;

        OnWaveStart?.Invoke(_currentWave);
    }
    private void GenerateEnemy()
    {
        if (_spawnPoints == null || _spawnPoints.Length == 0 || _enemyPrefab == null || _enemyPrefab.Length == 0)
        {
            Debug.LogError("Spawn points or enemy prefab not set in WaveController.");
            return;
        }
        int randomSpawnIndex = UnityEngine.Random.Range(0, _spawnPoints.Length);
        int randomEnemyPoolIndex = UnityEngine.Random.Range(0, _enemyPrefab.Length);

        EnemigoIngles enemigo = PoolManager.Instance.Pull(_enemyPrefab[randomEnemyPoolIndex],
                                                          _spawnPoints[randomSpawnIndex].position,
                                                          Quaternion.identity) as EnemigoIngles;
        enemigo.AddObservable(this);
        _waveEnemies--;
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
        _remainingEnemies--;

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
