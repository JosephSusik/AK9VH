using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpSpeed = 10f;
    private bool facingRight = true;
    private Vector2 moveInput;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction hurtAction;
    private InputAction jumpAction;
    private PlayerStats stats;

    [Header("Combat Logic")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        hurtAction = playerInput.actions["Hurt"];
        jumpAction = playerInput.actions["Jump"];

        attackAction.started += OnAttack;
        hurtAction.started += OnHurt;
        jumpAction.started += OnJump;

        stats = GetComponent<PlayerStats>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        attackAction.Enable();
        hurtAction.Enable();
        jumpAction.Enable();
    }

    private void OnDisable()
    {
        attackAction.started -= OnAttack;
        hurtAction.started -= OnHurt;
        jumpAction.started -= OnJump;
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();

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
        if (stats.UseStamina(50f))
        {
            animator.SetTrigger("IsAttacking");
            StartCoroutine(DelayedAttackCheck());
        }
    }

    IEnumerator DelayedAttackCheck()
    {
        yield return new WaitForSeconds(0.15f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        if (hits.Length == 0)
        {
            stats.TakeDamage(stats.attackDamage);
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
    private void OnAttack(InputAction.CallbackContext ctx) => Attack();
    private void OnHurt(InputAction.CallbackContext ctx) => Hurt();
    private void OnJump(InputAction.CallbackContext ctx) => Jump();

    private bool IsGrounded()
    {
        return rb.linearVelocity.y <= 0.01f;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
