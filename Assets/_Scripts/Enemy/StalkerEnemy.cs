using System.Collections;
using UnityEngine;

public class StalkerEnemy : Enemy
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;

    [SerializeField] private float stalkBuffer = 1f;
    [SerializeField] private float stalkWaitTime = 1f;
    [SerializeField] private float pouceForce = 10f;
    [SerializeField] private float pouceTime = 1f;
    [Range(0, 1)] [SerializeField] private float pouceEndMultiplier = 0.5f;

    private Transform targetPosition;

    private enum StalkerState {
        Approach,
        Stalk,
        Pounce,
        Chase
    }

    private StalkerState currentState = StalkerState.Approach;
    private bool isStalking;
    private bool isPouncing;

    protected override void Start() {
        base.Start();
        targetPosition = Player.Instance.transform;
    }

    private void Update() {
        HandleStalkerState();
        LookRotation();
    }

    protected override void Death() {
        base.Death();
        // Stalker Death Logic
        Destroy(gameObject);
    }

    private void HandleStalkerState() {
        switch (currentState) {
            case StalkerState.Approach:
                ApproachTarget();
                break;
            case StalkerState.Stalk:
                if (!isStalking)
                    StartCoroutine(StalkPlayer());
                break;
            case StalkerState.Pounce:
                if (!isPouncing)
                    StartCoroutine(PounceOnPlayer());
                break;
            case StalkerState.Chase:
                FollowTarget();
                break;
             default:
                break;
        }
    }

    private void ApproachTarget() {
        float distanceToSafeZoneEdge = Vector2.Distance(transform.position, SafeZone.Instance.transform.position) - SafeZone.Instance.transform.localScale.x / 2;

        if (distanceToSafeZoneEdge <= stalkBuffer) {
            rb.velocity = Vector2.zero; // stop movement
            currentState = StalkerState.Stalk;
        } else {
            Vector2 direction = (targetPosition.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    private IEnumerator StalkPlayer() {
        isStalking = true;
        yield return new WaitForSeconds(stalkWaitTime);
        currentState = StalkerState.Pounce;
        isStalking = false;
    }

    private IEnumerator PounceOnPlayer() {
        isPouncing = true;
        Vector2 direction = targetPosition.position - transform.position;
        rb.velocity = direction.normalized * pouceForce;
        yield return new WaitForSeconds(pouceTime);

        rb.velocity *= pouceEndMultiplier;
        currentState = StalkerState.Chase;
        isPouncing = false;
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