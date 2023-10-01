﻿using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;

    [SerializeField] private float damage;
    [SerializeField] private float safeZoneIncreaseValue;

    protected bool isInSafeZone;

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

    protected virtual void Death() {
        //Death Animation
        OnEnemyDeath?.Invoke(enemyType, isInSafeZone);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
            isInSafeZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.CompareTag("SafeZone")) {
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