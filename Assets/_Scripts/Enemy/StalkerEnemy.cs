using System.Collections;
using UnityEngine;

public class StalkerEnemy : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float stalkBuffer = 1f;
    [SerializeField] private float stalkWaitTime = 1f;
    [SerializeField] private float pouceForce = 10f;
    [SerializeField] private float pouceTime = 1f;
    [Range(0, 1)] [SerializeField] private float pouceEndMultiplier = 0.5f;
    [SerializeField] private float knockbackForce;

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private float shakeDuration;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] deathSounds;

    private Transform targetPosition;

    private enum StalkerState {
        Approach,
        Stalk,
        Pounce,
        Chase
    }

    private StalkerState currentState = StalkerState.Approach;
    private bool isStalking;
    private bool isPouncing;

    protected void Awake() => enemyType = EnemyType.Stalker;

    protected override void Start() {
        base.Start();
        enemyType = EnemyType.Stalker;
        targetPosition = Player.Instance.transform;
    }

    private void Update() {
        HandleStalkerState();
        LookRotation();
    }

    protected override void Death() {
        base.Death();
        // Stalker Death Logic
        PlayDeathSound();
        Destroy(gameObject);
    }

    private void HandleStalkerState() {
        switch (currentState) {
            case StalkerState.Approach:
                ApproachTarget();
                break;
            case StalkerState.Stalk:
                if (!isStalking)
                    StartCoroutine(StalkPlayer());
                break;
            case StalkerState.Pounce:
                if (!isPouncing)
                    StartCoroutine(PounceOnPlayer());
                break;
            case StalkerState.Chase:
                FollowTarget();
                break;
             default:
                break;
        }
    }

    private void ApproachTarget() {
        float distanceToSafeZoneEdge = Vector2.Distance(transform.position, SafeZone.Instance.transform.position) - SafeZone.Instance.transform.localScale.x / 2;

        if (distanceToSafeZoneEdge <= stalkBuffer) {
            rb.velocity = Vector2.zero; // stop movement
            currentState = StalkerState.Stalk;
        } else {
            Vector2 direction = (targetPosition.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    private IEnumerator StalkPlayer() {
        isStalking = true;
        yield return new WaitForSeconds(stalkWaitTime);
        currentState = StalkerState.Pounce;
        isStalking = false;
    }

    private IEnumerator PounceOnPlayer() {
        isPouncing = true;
        PlayAttackSound();
        Vector2 direction = targetPosition.position - transform.position;
        rb.velocity = direction.normalized * pouceForce;
        yield return new WaitForSeconds(pouceTime);

        rb.velocity *= pouceEndMultiplier;
        currentState = StalkerState.Chase;
        isPouncing = false;
    }

    private void FollowTarget() {
        Vector2 direciton = targetPosition.position - transform.position;
        rb.velocity = direciton.normalized * moveSpeed;
    }

    void LookRotation() {
        Vector2 lookDirection = targetPosition.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void PlayAttackSound() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(attackSounds[Random.Range(0, attackSounds.Length)], variablePitch);
    }

    private void PlayDeathSound() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(deathSounds[Random.Range(0, deathSounds.Length)], variablePitch);
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player")) {
            if (isPouncing) {
                Vector2 knockbackDirection = col.transform.position - transform.position;
                col.collider.GetComponent<Player>().Knockback(knockbackDirection, knockbackForce);
                
                CameraShake.Instance.TriggerCameraShake(
                shakeIntensity,
                shakeFrequency,
                shakeDuration);
            }
        }
    }
}