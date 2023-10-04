using System.Collections;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargerDespawnTime;
    [SerializeField] private float trailDespawnTime;
    [SerializeField] protected float knockbackForce;

    [SerializeField] private float shakeIntensity;
    [SerializeField] private float shakeFrequency;
    [SerializeField] private float shakeDuration;

    [SerializeField] private AudioClip spawnImpact;
    [SerializeField] private AudioClip warCry;

    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] deathSounds;

    private Transform targetPosition;
    private Vector2 chargeDirection;
    private bool canCharge;

    protected void Awake() => enemyType = EnemyType.Charger;

    protected override void Start() {
        base.Start();
        Destroy(gameObject, chargerDespawnTime);

        if (SoundManager.Instance == null) return;
        StartCoroutine(HandleSpawn());
    }

    private void Update() {
        Charge();
        HandleVoidTrail();
    }

    protected override void Death() {
        base.Death();
        // Charger Death Logic
        PlayDeathSound();
        Destroy(gameObject);
    }

    private void Charge() {
        if (canCharge) rb.velocity = chargeDirection * chargeSpeed;
    }

    private void HandleVoidTrail() {

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