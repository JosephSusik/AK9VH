using UnityEngine;
using UnityEngine.UI;

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

    [Header("UI")]
    public Image healthBarFill;

    private Rigidbody2D rb;
    private Animator animator;
    private float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;
        UpdateHealthBar();

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
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Enemy hp: {currentHealth}");

        UpdateHealthBar();

        animator.SetTrigger("isHurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = currentHealth / maxHealth;
    }

    private void Die()
    {
        if (PlayerStats.Instance != null)
        {
            PlayerStats.Instance.AddUpgradePoints(2);
        }

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
        if (playerStats == null) return;

        // Is player to the right of the enemy?
        bool playerOnRight = collision.transform.position.x > transform.position.x;

        // Check if enemy is facing the player
        bool facingPlayer =
            (facingRight && playerOnRight) ||
            (!facingRight && !playerOnRight);

        if (facingPlayer)
        {
            animator.SetTrigger("isAttacking");
            playerStats.TakeDamage(damage);
        }
    }
}
}
