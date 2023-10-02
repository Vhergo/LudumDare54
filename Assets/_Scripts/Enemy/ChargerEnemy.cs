using System.Collections;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargerDespawnTime;

    [SerializeField] private float trailDespawnTime;

    [SerializeField] protected float knockbackForce;


    private Transform targetPosition;
    private Vector2 chargeDirection;

    protected override void Start() {
        base.Start();
        targetPosition = Player.Instance.transform;
        chargeDirection = (targetPosition.position - transform.position).normalized;

        SetRotation();
        Destroy(gameObject, chargerDespawnTime);
    }

    private void Update() {
        Charge();
        HandleVoidTrail();
    }

    protected override void Death() {
        base.Death();
        // Charger Death Logic
        Destroy(gameObject);
    }

    private void Charge() {
        rb.velocity = chargeDirection * chargeSpeed;
    }

    private void HandleVoidTrail() {

    }

    void SetRotation() {
        Vector2 lookDirection = targetPosition.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); ;
    }

    protected override void OnCollisionEnter2D(Collision2D col) {
        base.OnCollisionEnter2D(col);
        if (col.collider.CompareTag("Player")) {
            Vector2 knockbackDirection = col.transform.position - transform.position;
            col.collider.GetComponent<Player>().Knockback(knockbackDirection, knockbackForce);

            GetComponent<Collider2D>().isTrigger = true;
        }
    }
}