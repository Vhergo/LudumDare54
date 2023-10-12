using System.Collections;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    [SerializeField] private float rotationSpeed = 10f;

    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargerDespawnTime;
    [SerializeField] private float trailDespawnTime;
    [SerializeField] protected float knockbackForce;
    [Range(0, 1)] [SerializeField] private float haltThreshold;

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private float shakeDuration;

    [SerializeField] private AudioClip spawnImpact;
    [SerializeField] private AudioClip warCry;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] deathSounds;

    private Vector2 chargeDirection;
    private bool canCharge;
    private bool isHalted;

    protected override void Start() {
        base.Start();
        Destroy(gameObject, chargerDespawnTime);

        SetInitialRotation();
        StartCoroutine(HandleSpawn());
    }

    private void Update() {
        Attack();
    }

    private void Attack() {
        if (!canCharge) return;

        if (isHalted) {
            FollowTarget();
            HandleRotation();
        } else {
            Charge();
        }
    }

    public override void TakeDamage(float damageTaken) {
        base.TakeDamage(damageTaken);
        if (currentHealth <= maxHealth * haltThreshold) {
            isHalted = true;
            PlayAttackSound();
        }
    }

    private void Charge() {
        rb.velocity = chargeDirection * chargeSpeed;
    }

    private void FollowTarget() {
        Vector2 direction = (targetPosition.position - transform.position).normalized;
        Vector2 targetVelocity = direction * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        if (!canAttack) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    protected override void Death() {
        base.Death();
        // Charger Death Logic
        PlayDeathSound();
        Destroy(gameObject);
    }
    void SetInitialRotation() {
        Vector2 lookDirection = targetPosition.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); ;
    }

    void HandleRotation() {
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

    private IEnumerator HandleSpawn() {
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(spawnImpact);
        yield return new WaitForSeconds(spawnImpact.length / 2);
        if (SoundManager.Instance != null) SoundManager.Instance.PlaySound(warCry);
        yield return new WaitForSeconds(warCry.length / 2);

        targetPosition = Player.Instance.transform;
        chargeDirection = (targetPosition.position - transform.position).normalized;

        canCharge = true;
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player")) {
            Vector2 knockbackDirection = col.transform.position - transform.position;
            col.collider.GetComponent<Player>().Knockback(knockbackDirection, knockbackForce);
            PlayAttackSound();
            CameraShake.Instance.TriggerCameraShake(
                shakeIntensity,
                shakeFrequency,
                shakeDuration);
            GetComponent<Collider2D>().isTrigger = true;
        }
    }
}