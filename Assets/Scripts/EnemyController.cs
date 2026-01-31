using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveTime = 2f;

    private bool facingRight = true;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        timer = moveTime;
        animator.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        // Determine direction based on facing
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        // Move enemy
        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Countdown to flip
        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Flip();
            timer = moveTime;
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
