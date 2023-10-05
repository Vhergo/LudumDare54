using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;

    [SerializeField] protected float damage;
    [SerializeField] protected float attackSpeed;

    [SerializeField] protected float selfKnockbackForce;

    [SerializeField] protected AudioClip[] hitSounds;
    [SerializeField] protected bool variablePitch;

    [SerializeField] private ParticleSystem bloodParticle;
    private Transform bloodContainer;

    protected Transform targetPosition;

    protected bool isInSafeZone;
    protected bool canAttack = true;
    protected bool isAlive = true;

    protected Rigidbody2D rb;
    public EnemyType enemyType;

    public static event Action<GameObject, EnemyType, bool> OnEnemyDeath;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        targetPosition = Player.Instance.transform;
        bloodContainer = GameObject.FindGameObjectWithTag("BloodContainer").transform;
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageTaken) {
        if (!isAlive) return;

        PlayHitSound();
        currentHealth -= damageTaken;
        if (currentHealth <= 0) {
            Death();
        }
    }

    protected virtual void Death() {
        //Death Animation
        isAlive = false;
        HandleBloodOnDeath();

        OnEnemyDeath?.Invoke(gameObject, enemyType, isInSafeZone);
    }

    private void HandleBloodOnDeath() {
        bloodParticle.Play();
        bloodParticle.transform.SetParent(bloodContainer);
    }

    private IEnumerator DealDamage() {
        canAttack = false;
        Player.Instance.TakeDamage(damage, enemyType);
        yield return new WaitForSeconds(attackSpeed);
        canAttack = true;
    }

    public virtual void Knockback() {
        Vector2 knockbackDirection = (transform.position - Player.Instance.transform.position).normalized;
        rb.AddForce(knockbackDirection * selfKnockbackForce, ForceMode2D.Impulse);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col) {
        if (col.collider.CompareTag("Player")) {
            if (canAttack) StartCoroutine(DealDamage());
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
            isInSafeZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
            isInSafeZone = false;
        }
    }

    private void PlayHitSound() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(hitSounds[UnityEngine.Random.Range(0, hitSounds.Length)]);
    }
}

public enum EnemyType
{
    // Grunts: Default enemy types
    Grunt,

    // Stalkers: The stay near the edge of the darkness and pounch at you
    Stalker,

    // Chargers: They just charge in a straight line at you
    Charger,

    // Strech Goal
    // Harginger: they create dark zones that extend the void
    // Stepping on dark zones will damage you
    Harbinger,
    Void
}