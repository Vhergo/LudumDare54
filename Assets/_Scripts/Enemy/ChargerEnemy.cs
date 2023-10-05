using System.Collections;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

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

        if (SoundManager.Instance == null) return;
        StartCoroutine(HandleSpawn());
    }

    private void Update() {
        Attack();
    }

    public override void TakeDamage(float damageTaken) {
        base.TakeDamage(damageTaken);
        if (currentHealth <= maxHealth * haltThreshold) {
            isHalted = true;
            PlayAttackSound();
        }
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

    private void Attack() {
        if (!canCharge) return;
        
        if (isHalted) {
            FollowTarget();
        } else {
            Charge();
        }
    }

    private void Charge() {
        rb.velocity = chargeDirection * chargeSpeed;
    }

    void SetRotation() {
        Vector2 lookDirection = targetPosition.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); ;
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
        SoundManager.Instance.PlaySound(spawnImpact);
        yield return new WaitForSeconds(spawnImpact.length / 2);
        SoundManager.Instance.PlaySound(warCry);
        yield return new WaitForSeconds(warCry.length / 2);

        targetPosition = Player.Instance.transform;
        chargeDirection = (targetPosition.position - transform.position).normalized;
        SetRotation();

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