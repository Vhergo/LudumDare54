using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public static SafeZone Instance { get; private set; }

    [SerializeField] private Vector3 initialScale;
    [Range(0, 1)] public float shrinkSpeed = 0.2f;

    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageCooldown = 5f;
    private float damageTimer;
    private bool playerInVoid;

    [SerializeField] private float gruntSafeZoneBonus;
    [SerializeField] private float stalkerSafeZoneBonus;
    [SerializeField] private float chargerSafeZoneBonus;
    [SerializeField] private float harbingerSafeZoneBonus;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() {
        Enemy.OnEnemyDeath += GrowSafeZone;
    }

    private void OnDisable() {
        Enemy.OnEnemyDeath -= GrowSafeZone;
    }

    private void Start() {
        transform.localScale = initialScale;
        damageTimer = 0;
    }

    private void Update() {
        ShrinkSafeZone();
        DamagePlayer();
    }

    private void ShrinkSafeZone() {
        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
    }

    [ContextMenu("Grow Safe Zone")]
    public void GrowSafeZone(EnemyType enemyType) {
        transform.localScale += Vector3.one * ChooseEnemySafeZoneBonus(enemyType);
    }

    private float ChooseEnemySafeZoneBonus(EnemyType enemyType) {
        switch (enemyType) {
            case EnemyType.Grunt:
                return gruntSafeZoneBonus;
            case EnemyType.Stalker:
                return stalkerSafeZoneBonus;
            case EnemyType.Charger:
                return chargerSafeZoneBonus;
            case EnemyType.Harbinger:
                return harbingerSafeZoneBonus;
            default:
                return 0;

        }
    }

    private void DamagePlayer() {
        if (playerInVoid) {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldown) {
                Player.Instance.TakeDamage(damage, EnemyType.Void);
                damageTimer = 0;
            }
        }
    }

    // Out of safe zone means you're in the void and can take damage
    private void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            playerInVoid = true;
            damageTimer = 0;
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Player")) {
            playerInVoid = false;
            damageTimer = 0;
        }
    }
}
