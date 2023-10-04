using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeZone : MonoBehaviour
{
    public static SafeZone Instance { get; private set; }

    [SerializeField] private Vector3 initialScale;
    [Range(0, 1)] public float shrinkSpeed = 0.2f;
    private bool canShrink = true;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveRange = 50f;
    [SerializeField] private float moveCooldown = 5f;
    private Vector2 targetPosition;
    private bool isMoving;

    [SerializeField] private float damage = 10f;
    [SerializeField] private float damageCooldown = 5f;
    private float damageTimer;
    private bool playerInVoid;

    [Header("Safe Zone Bonuses")]
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
        HandleSafeZoneMovement();
    }

    private void HandleSafeZoneMovement() {
        if (!isMoving && Vector2.Distance(transform.position, targetPosition) <= 0.1f) {
            targetPosition = GetRandomPointInRange();
            StartCoroutine(MoveSafeZone());
        }
    }

    private IEnumerator MoveSafeZone() {
        isMoving = true;
        while ((Vector2)transform.position != targetPosition) {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // wait for next frame
        }
        yield return new WaitForSeconds(moveCooldown);
        isMoving = false;
    }

    private Vector2 GetRandomPointInRange() {
        return Vector2.zero + Random.insideUnitCircle * moveRange;
    }

    private void ShrinkSafeZone() {
        if (!canShrink) return;

        transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
        if (transform.transform.localScale.x <= 0) canShrink = false;
    }

    public void GrowSafeZone(GameObject enemy, EnemyType enemyType, bool enemyDiedInSafeZone) {
        if (enemyDiedInSafeZone)
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
