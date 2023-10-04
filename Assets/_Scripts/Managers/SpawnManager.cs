using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private WeightedEnemy[] spawnList;
    [SerializeField] private List<WeightedEnemy> activeSpawnList = new List<WeightedEnemy>();
    private List<GameObject> spawnPool = new List<GameObject>();

    [SerializeField] private float spawnRadiusBuffer = 10f;

    [SerializeField] private Transform enemiesParent;

    [SerializeField] private List<WaveData> waveData;
    public float waveTimer;
    private WaveData currentWaveData;
    private int currentWave = 1;
    private int enemyPrefabIndex = 0;
    private bool allEnemiesAdded;

    public static Action<int> OnWaveChange;
    public static Action<GameObject> OnChargerSpawn;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            activeSpawnList.Add(spawnList[enemyPrefabIndex++]);
            InitializeSpawnPool();
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        if (spawnPool.Count <= 0) return;

        InitializeWaves();
        StartCoroutine(SpawnRoutine());
    }

    private void Update() {
        HandleWaves();
    }

    private void InitializeWaves() {
        currentWaveData = waveData[0];
        waveTimer = currentWaveData.waveDuration;
        OnWaveChange?.Invoke(currentWave);
    }

    private void InitializeSpawnPool() {
        spawnPool.Clear();
        foreach (WeightedEnemy enemy in activeSpawnList) {
            for (int i = 0; i < enemy.weight; i++) {
                spawnPool.Add(enemy.enemyPrefab);
            }
        }
    }

    private IEnumerator SpawnRoutine() {
        while (!Player.Instance.isDead) {
            yield return new WaitForSeconds(currentWaveData.spawnDelay);
            GameObject spawnedEnemy = Instantiate(GetRandomEnemy(), GetSpawnPosition(), Quaternion.identity, enemiesParent);

            if (spawnedEnemy.GetComponent<Enemy>().enemyType == EnemyType.Charger) {
                OnChargerSpawn?.Invoke(gameObject);
                Debug.Log("Charger Spawned");
            }
        }
    }

    private void HandleWaves() {
        if (waveTimer < 0) {
            waveTimer = currentWaveData.waveDuration;
            currentWave++;
            OnWaveChange?.Invoke(currentWave);
            IncreaseDifficulty();
        } else {
            waveTimer -= Time.deltaTime;
        }
    }

    private void IncreaseDifficulty() {
        foreach (WaveData wave in waveData) {
            if (currentWave == wave.waveNumber) {
                currentWaveData = wave;

                if (!allEnemiesAdded) activeSpawnList.Add(spawnList[enemyPrefabIndex]);
                if (enemyPrefabIndex < spawnList.Length - 1) {
                    enemyPrefabIndex++;
                } else {
                    allEnemiesAdded = true;
                }

                InitializeSpawnPool();
                break;
            }
        }
    }

    private GameObject GetRandomEnemy() {
        return spawnPool[UnityEngine.Random.Range(0, spawnPool.Count)];
    }

    private Vector2 GetSpawnPosition() {
        float safeZoneRadius = SafeZone.Instance.transform.localScale.x / 2;
        float offset = safeZoneRadius + spawnRadiusBuffer;
        Vector2 spawnPosition = UnityEngine.Random.insideUnitCircle.normalized * offset;
        return (Vector2)SafeZone.Instance.transform.position + spawnPosition;
    }

    public int GetWave() {
        return currentWave;
    }
}

[Serializable]
public class WeightedEnemy
{
    public string enemyName;
    public GameObject enemyPrefab;
    public int weight;
}

[Serializable]
public class WaveData
{
    public int waveNumber;
    public float waveDuration;
    public float spawnDelay;
}
