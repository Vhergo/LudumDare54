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

    protected bool isInSafeZone;
    protected bool canAttack = true;

    protected Rigidbody2D rb;
    protected EnemyType enemyType;

    public static event Action<EnemyType, bool> OnEnemyDeath;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float damageTaken) {
        currentHealth -= damageTaken;
        if (currentHealth <= 0) {
            Death();
        }
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

    protected virtual void Death() {
        //Death Animation
        OnEnemyDeath?.Invoke(enemyType, isInSafeZone);
    }

    protected virtual void OnCollisionEnter2D(Collision2D col) {
        if (col.collider.CompareTag("Player")) {
            if (canAttack) StartCoroutine(DealDamage());
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
            Debug.Log("Enter Safe Zone");
            isInSafeZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
            Debug.Log("Exit Safe Zone");
            isInSafeZone = false;
        }
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