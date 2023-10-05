using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GruntEnemy : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;

    [SerializeField] private float rotationSpeed;

    [SerializeField] private AudioClip[] deathSounds;

    protected override void Start() {
        base.Start();
        // Grunt Start Logic
    }

    private void Update() => LookRotation();

    private void FixedUpdate() => FollowTarget();

    protected override void Death() {
        base.Death();
        // Grunt Death Logic
        PlayDeathSound();
        Destroy(gameObject);
    }

    private void FollowTarget() {
        Vector2 direction = (targetPosition.position - transform.position).normalized;
        Vector2 targetVelocity = direction * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);

        if (!canAttack) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    void LookRotation() {
        Vector2 lookDirection = targetPosition.position - transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }

    private void PlayDeathSound() {
        if (SoundManager.Instance == null) return;
        SoundManager.Instance.PlaySound(deathSounds[Random.Range(0, deathSounds.Length)], variablePitch);
    }
}