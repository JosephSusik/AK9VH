using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private bool facingRight = true;
    private Vector2 moveInput;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction attackAction;
    private InputAction hurtAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component not found! Add it to this GameObject.");
            return;
        }

        // Link actions from your PlayerControls asset
        moveAction = playerInput.actions["Move"];
        attackAction = playerInput.actions["Attack"];
        hurtAction = playerInput.actions["Hurt"];

        // Subscribe to single-press events
        attackAction.started += ctx => Attack();
        hurtAction.started += ctx => Hurt();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        attackAction.Enable();
        hurtAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        attackAction.Disable();
        hurtAction.Disable();
    }

    private void Update()
    {
        // Movement
        moveInput = moveAction.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);

        // Flip character
        if (moveInput.x > 0 && !facingRight) Flip();
        else if (moveInput.x < 0 && facingRight) Flip();

        // Animations
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

    private void Attack() => animator.SetTrigger("IsAttacking");
    private void Hurt() => animator.SetTrigger("IsHurt");

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
