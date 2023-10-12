using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement Instance { get; private set; }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float deceleration = 1f;
    private Vector2 moveDirection;
    private Vector2 mousePos;

    [SerializeField] private float dashForce = 12f;
    [SerializeField] private float dashDuration = 1f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing;

    private Rigidbody2D rb;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnEnable() => Player.OnPlayerDeath += PlayerDeath;
    private void OnDisable() => Player.OnPlayerDeath -= PlayerDeath;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        ProcessInput();
    }

    private void FixedUpdate() {
        if (Player.Instance.isDead) return;
        Move();
    }

    private void ProcessInput() {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;

        if (Input.GetKey(KeyCode.Space) && !isDashing) {
            StartCoroutine(Dash());
        }
    }

    private void Move() {
        Vector2 targetVelocity = moveDirection * moveSpeed;
        rb.velocity = Vector2.Lerp(rb.velocity, targetVelocity, acceleration * Time.fixedDeltaTime);
        if (moveDirection == Vector2.zero) {
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
        }
    }

    private IEnumerator Dash() {
        isDashing = true;
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseDirection = (mousePos - (Vector2)transform.position).normalized;
        Vector2 dashDirection = (moveDirection == Vector2.zero) ? mouseDirection : moveDirection;
        rb.velocity = dashDirection * dashForce;

        yield return new WaitForSeconds(dashDuration);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(dashCooldown);
        isDashing = false;

    }

    private void PlayerDeath(EnemyType enemyType) {
        rb.velocity = Vector2.zero;
    }
}
