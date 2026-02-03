using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : BaseController
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    private bool facingRight = true;
    private Vector2 moveInput;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Combat Logic")]
    public Transform attackPoint;
    public float attackRange = 1f;
    public LayerMask enemyLayers;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.1f;
    public LayerMask groundLayer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return; // Ignore input when game is paused

        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }
    private void Attack()
    {
        if (Time.timeScale == 0f) return; // Ignore input when game is paused

        if (PlayerStats.Instance != null && PlayerStats.Instance.UseStamina(50f))
        {
            animator.SetTrigger("IsAttacking");
            StartCoroutine(DelayedAttackCheck());
        }
    }

    IEnumerator DelayedAttackCheck()
    {
        yield return new WaitForSeconds(0.15f);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        if (hits.Length == 0)
        {
            PlayerStats.Instance.ResetCombo();
            PlayerStats.Instance.TakeDamage(PlayerStats.Instance.attackDamage);
        }
        else
        {
            PlayerStats.Instance.RegisterHit();
        }

            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out EnemyController enemy))
                {
                    float finalDamage = PlayerStats.Instance.attackDamage * PlayerStats.Instance.comboMultiplier;
                    Debug.Log($"Enemy hit: {enemy.name} took {finalDamage} damage");
                    enemy.TakeDamage(finalDamage);
                }
            }
    }

    private void Hurt()
    {
        animator.SetTrigger("IsHurt");
    }

    private void Jump()
    {
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }
    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnJump(InputValue value) { if (value.isPressed) Jump(); }
    public void OnAttack(InputValue value) { if (value.isPressed) Attack(); }
    public void OnHurt(InputValue value) { if (value.isPressed) Hurt(); }

    // private bool IsGrounded()
    // {
    //     return rb.linearVelocity.y <= 0.01f;
    // }
    private bool IsGrounded()
    {
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    // Optional: Visualize in editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
