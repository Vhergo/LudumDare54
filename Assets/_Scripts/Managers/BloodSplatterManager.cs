using System.Collections;
using UnityEngine;

public class BloodSplatterManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }

    private void OnEnable() {
        Enemy.OnEnemyDeath += SpawnBloodSplatter;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= SpawnBloodSplatter;
    }

    private void SpawnBloodSplatter(GameObject enemy, EnemyType enemyType, bool x) {

    }
}