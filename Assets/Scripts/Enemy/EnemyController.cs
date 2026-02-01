using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float patrolTime = 2f;
    private bool facingRight = true;

    [Header("AI Behavior")]
    public float detectionRange = 8f;
    public float attackRange = 1.5f;
    public float losePlayerRange = 12f;
    private Transform player;
    private enum EnemyState { Patrol, Chase, Attack }
    private EnemyState currentState = EnemyState.Patrol;

    [Header("Combat")]
    public float maxHealth = 50f;
    private float currentHealth;
    public float damage = 35f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("UI")]
    public Image healthBarFill;

    private Rigidbody2D rb;
    private Animator animator;
    private float patrolTimer;
    private float currentSpeed;
    private PlayerStats targetPlayer; // Store reference to player being attacked

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthBar();
        patrolTimer = patrolTime;
        currentSpeed = patrolSpeed;
        
        // Freeze rotation, so the enemy doesn't tumble
        rb.freezeRotation = true;
        
        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        
        animator.SetBool("isWalking", true);
    }

    void Update()
    {
        if (currentHealth <= 0) return;

        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // State transitions
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (distanceToPlayer <= detectionRange)
                {
                    currentState = EnemyState.Chase;
                    currentSpeed = chaseSpeed;
                }
                break;

            case EnemyState.Chase:
                // The 0.2 key, without it distanceToPlayer is too big, even tho they colide
                if (distanceToPlayer <= attackRange + 0.2)
                {
                    currentState = EnemyState.Attack;
                    currentSpeed = 0f;
                }
                else if (distanceToPlayer > losePlayerRange)
                {
                    currentState = EnemyState.Patrol;
                    currentSpeed = patrolSpeed;
                }
                break;

            case EnemyState.Attack:
                if (distanceToPlayer > attackRange)
                {
                    currentState = EnemyState.Chase;
                    currentSpeed = chaseSpeed;
                }
                break;
        }
    }

    void FixedUpdate()
    {
        if (currentHealth <= 0) return;

        switch (currentState)
        {
            case EnemyState.Patrol:
                Patrol();
                break;

            case EnemyState.Chase:
                ChasePlayer();
                break;

            case EnemyState.Attack:
                AttackPlayer();
                break;
        }
    }

    private void Patrol()
    {
        // Move horizontally only, preserve vertical velocity (gravity)
        float horizontalMovement = (facingRight ? 1f : -1f) * patrolSpeed;
        rb.linearVelocity = new Vector2(horizontalMovement, rb.linearVelocity.y);

        patrolTimer -= Time.fixedDeltaTime;
        if (patrolTimer <= 0f)
        {
            Flip();
            patrolTimer = patrolTime;
        }

        animator.SetBool("isWalking", true);
    }

    private void ChasePlayer()
    {
        if (player == null) return;

        // Calculate horizontal direction to player
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        
        // Move horizontally only, preserve vertical velocity (gravity)
        rb.linearVelocity = new Vector2(directionX * chaseSpeed, rb.linearVelocity.y);

        // Face the player
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            Flip();
        }

        animator.SetBool("isWalking", true);
    }

    private void AttackPlayer()
    {
        if (player == null) return;

        // Stop horizontal movement, preserve vertical velocity (gravity)
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);

        // Face the player
        float directionX = player.position.x - transform.position.x;
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            Flip();
        }

        // Attack with cooldown
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            animator.SetTrigger("isAttacking");
            
            // PlayerStats playerStats = player.GetComponent<PlayerStats>();
            // if (playerStats != null)
            // {
            //     playerStats.TakeDamage(damage);
            // }
            
            // lastAttackTime = Time.time;
            // Store player reference for when animation finishes
            targetPlayer = player.GetComponent<PlayerStats>();
            
            lastAttackTime = Time.time;
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
        else
        {
            // When hit, switch to chase if not already
            if (currentState == EnemyState.Patrol)
            {
                currentState = EnemyState.Chase;
                currentSpeed = chaseSpeed;
            }
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
        Debug.Log($"enemy dies hp: {currentHealth}");
        animator.SetTrigger("isDead");
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        this.enabled = false;
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    //Draw detection ranges in Scene view
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    public void DealDamage()
    {
        if (targetPlayer != null)
        {
            targetPlayer.TakeDamage(damage);
        }
    }
}