using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorManager : MonoBehaviour
{
    public static IndicatorManager Instance { get; private set; }

    [SerializeField] private GameObject indicatorIconPrefab;
    [SerializeField] private Transform indicatorParent;

    private Dictionary<GameObject, GameObject> chargerToIndicator = new Dictionary<GameObject, GameObject>();

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        SpawnManager.OnChargerSpawn += CreateIndicatorIcon;
        Enemy.OnEnemyDeath += RemoveIndicator;
    }

    private void OnDisable() {
        SpawnManager.OnChargerSpawn -= CreateIndicatorIcon;
        Enemy.OnEnemyDeath += RemoveIndicator;
    }

    private void CreateIndicatorIcon(GameObject charger) {
        GameObject indicatorIcon = Instantiate(indicatorIconPrefab, transform.position, Quaternion.identity, indicatorParent);
        chargerToIndicator[charger] = indicatorIcon;
        indicatorIcon.GetComponent<Indicator>().SetTarget(charger.transform);
    }

    private void RemoveIndicator(GameObject charger, EnemyType enemyType, bool isInSafeZone) {
        if (enemyType == EnemyType.Charger) {
            if (chargerToIndicator.TryGetValue(charger, out GameObject indicatorIcon)) {
                Destroy(indicatorIcon);
                chargerToIndicator.Remove(charger);
            }
        }
    }
}