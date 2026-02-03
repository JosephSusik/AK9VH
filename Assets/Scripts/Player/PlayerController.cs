using UnityEngine;
using UnityEngine.InputSystem;

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
    private float attackCooldown = 0.5f;
    private float lastAttackTime;
    private bool isAttacking = false;

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
        if (Time.timeScale == 0f) return;

        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();

        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

    private void FixedUpdate()
    {
        if (Time.timeScale == 0f) return;
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
    }

    private void Attack()
    {
        if (Time.timeScale == 0f) return;
        if (isAttacking) return; // Prevent attack spam during animation
        if (Time.time < lastAttackTime + attackCooldown) return;

        if (PlayerStats.Instance != null && PlayerStats.Instance.UseStamina(50f))
        {
            isAttacking = true;
            lastAttackTime = Time.time;
            animator.SetTrigger("IsAttacking");
        }
    }

    // Call this from Animation Event at the moment of impact (e.g., frame 3-5)
    public void PerformAttackHitCheck()
    {
        if (attackPoint == null)
        {
            Debug.LogError("Attack Point not assigned!");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayers
        );

        if (hits.Length == 0)
        {
            // Missed attack
            PlayerStats.Instance.ResetCombo();
            PlayerStats.Instance.TakeDamage(PlayerStats.Instance.attackDamage);
        }
        else
        {
            // Hit enemy(ies)
            foreach (Collider2D hit in hits)
            {
                if (hit.TryGetComponent(out EnemyController enemy))
                {
                    PlayerStats.Instance.RegisterHit();
                    float finalDamage = PlayerStats.Instance.attackDamage * PlayerStats.Instance.comboMultiplier;
                    Debug.Log($"Enemy hit: {enemy.name} took {finalDamage} damage");
                    enemy.TakeDamage(finalDamage);
                }
            }
        }
    }

    // Call this from Animation Event at the end of attack animation
    public void OnAttackAnimationEnd()
    {
        isAttacking = false;
    }

    private void Hurt()
    {
        animator.SetTrigger("IsHurt");
    }

    private void Jump()
    {
        if (Time.timeScale == 0f) return;
        
        if (IsGrounded())
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpSpeed);
        }
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();
    public void OnJump(InputValue value) { if (value.isPressed) Jump(); }
    public void OnAttack(InputValue value) { if (value.isPressed) Attack(); }
    public void OnHurt(InputValue value) { if (value.isPressed) Hurt(); }

    private bool IsGrounded()
    {
        if (groundCheck == null)
        {
            Debug.LogError("Ground Check Transform not assigned!");
            return false;
        }
        return Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize ground check
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * groundCheckDistance);
        }

        // Visualize attack range
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
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