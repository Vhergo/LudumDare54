using System.Collections;
using UnityEngine;

public class GruntEnemy : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    private Transform targetPosition;

    protected override void Start() {
        base.Start();
        targetPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        FollowTarget();
        LookRotation();
    }

    protected override void Death() {
        base.Death();
        // Grunt Death Logic
        Destroy(gameObject);
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
}