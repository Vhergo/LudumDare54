using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public float maxHealth = 100f;
    public float currentHealth;

    public bool isDead;

    public static Action<EnemyType> OnPlayerDeath;
    public static Action<float> OnPlayerTakeDamage;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Start() {
        currentHealth = maxHealth;
        OnPlayerTakeDamage?.Invoke(currentHealth);
    }

    public void TakeDamage(float damageTaken, EnemyType enemyType) {
        currentHealth -= damageTaken;
        OnPlayerTakeDamage?.Invoke(currentHealth);
        if (currentHealth <= 0) {
            Death(enemyType);
        }
    }

    private void Death(EnemyType enemyType) {
        //Death Animation
        isDead = true;
        OnPlayerDeath?.Invoke(enemyType);
        Destroy(gameObject, 5f);
    }
}
