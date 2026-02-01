using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float moveTime = 2f;

    private bool facingRight = true;

    [Header("Combat")]
    public float maxHealth = 50f;
    private float currentHealth;
    public float damage = 35f;

    [Header("Components")]
    private Rigidbody2D rb;
    private Animator animator;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        timer = moveTime;
        animator.SetBool("isWalking", true);
    }

    void FixedUpdate()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            Flip();
            timer = moveTime;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"Enemy hp: {currentHealth}");

        animator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        animator.SetTrigger("isDead");
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 0.6f);
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerStats = collision.gameObject.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.TakeDamage(damage);
            }
        }
    }
}
