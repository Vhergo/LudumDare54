using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Bullet : MonoBehaviour
{
    [HideInInspector] public float damage;
    [HideInInspector] public float bulletLifeTime;

    private void Start() {
        Destroy(gameObject, bulletLifeTime);
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if (col.CompareTag("Enemy")) {
            col.GetComponent<Enemy>().TakeDamage(damage);
            col.GetComponent<Enemy>().Knockback();
            Destroy(gameObject);
        }
    }
}