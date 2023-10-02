using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    [SerializeField] private WeightedEnemy[] spawnList;
    private List<GameObject> spawnPool = new List<GameObject>();

    [SerializeField] private float spawnDelay = 1f;
    [SerializeField] private float spawnRadiusBuffer = 10f;

    [SerializeField] private Transform enemiesParent;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            InitializeSpawnPool();
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        StartCoroutine(SpawnRoutine());
    }

    private void InitializeSpawnPool() {
        spawnPool.Clear();
        foreach (WeightedEnemy enemy in spawnList) {
            for (int i = 0; i < enemy.weight; i++) {
                spawnPool.Add(enemy.enemyPrefab);
            }
        }
    }

    private IEnumerator SpawnRoutine() {
        while (true) {
            yield return new WaitForSeconds(spawnDelay);
            GameObject spawnedEnemy = Instantiate(GetRandomEnemy(), GetSpawnPosition(), Quaternion.identity, enemiesParent);
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
}

[Serializable]
public class WeightedEnemy
{
    public string enemyName;
    public GameObject enemyPrefab;
    public int weight;
}
