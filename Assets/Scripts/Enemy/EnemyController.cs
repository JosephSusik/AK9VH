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
    public float optimalAttackDistance = 1.2f; // Preferred distance to maintain from player
    private Transform player;
    private enum EnemyState { Patrol, Chase, Attack, PrepareAttack, Recover }
    private EnemyState currentState = EnemyState.Patrol;

    [Header("Combat")]
    public float maxHealth = 50f;
    private float currentHealth;
    public float damage = 35f;
    public float attackCooldown = 1.5f;
    public float initialAttackDelay = 0.5f; // Delay before first attack when entering range
    private float lastAttackTime;
    private bool isFirstAttackInSession = true; // Track if this is first attack in current session

    [Header("Combat Timing")]
    public float prepareAttackDuration = 0.3f; // Time to "wind up" before attacking
    public float recoveryDuration = 0.6f; // Time to back off after attacking
    public float recoveryBackoffDistance = 0.8f; // How far to back away during recovery
    
    private float stateTimer; // Timer for timed states
    private bool isAttacking = false; // Track if currently in attack animation

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
        
        lastAttackTime = -999f; // Initialize to very old time
        
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

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player == null || !player.gameObject.activeInHierarchy)
        {
            return;
        }

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
                // Enter prepare attack state when in range
                if (distanceToPlayer <= attackRange + 0.2f)
                {
                    // Check if enough time has passed since last attack
                    float requiredCooldown = isFirstAttackInSession ? initialAttackDelay : attackCooldown;
                    
                    if (Time.time >= lastAttackTime + requiredCooldown)
                    {
                        currentState = EnemyState.PrepareAttack;
                        stateTimer = prepareAttackDuration;
                        currentSpeed = 0f;
                    }
                    else
                    {
                        // Still cooling down, maintain distance
                        currentSpeed = chaseSpeed * 0.3f; // Slow approach
                    }
                }
                else if (distanceToPlayer > losePlayerRange)
                {
                    currentState = EnemyState.Patrol;
                    currentSpeed = patrolSpeed;
                    isFirstAttackInSession = true; // Reset for next encounter
                }
                break;

            case EnemyState.PrepareAttack:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    // Transition to actual attack
                    currentState = EnemyState.Attack;
                    isAttacking = true;
                }
                else if (distanceToPlayer > attackRange + 0.5f)
                {
                    // Player moved away during preparation
                    currentState = EnemyState.Chase;
                    currentSpeed = chaseSpeed;
                }
                break;

            case EnemyState.Attack:
                // Attack state handles itself, transitions in FixedUpdate
                break;

            case EnemyState.Recover:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0f)
                {
                    // Recovery complete, decide next state
                    if (distanceToPlayer <= attackRange + 0.5f)
                    {
                        currentState = EnemyState.Chase;
                        currentSpeed = chaseSpeed;
                    }
                    else if (distanceToPlayer > losePlayerRange)
                    {
                        currentState = EnemyState.Patrol;
                        currentSpeed = patrolSpeed;
                        isFirstAttackInSession = true;
                    }
                    else
                    {
                        currentState = EnemyState.Chase;
                        currentSpeed = chaseSpeed;
                    }
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

            case EnemyState.PrepareAttack:
                PrepareAttack();
                break;

            case EnemyState.Attack:
                ExecuteAttack();
                break;

            case EnemyState.Recover:
                RecoverFromAttack();
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Calculate horizontal direction to player
        float directionX = Mathf.Sign(player.position.x - transform.position.x);
        
        // Smart positioning: slow down when getting close to optimal distance
        float moveSpeed = chaseSpeed;
        if (distanceToPlayer < optimalAttackDistance + 0.3f)
        {
            // Very close, move slower or back off slightly
            moveSpeed = chaseSpeed * 0.4f;
            
            // If too close, actually back away
            if (distanceToPlayer < optimalAttackDistance * 0.8f)
            {
                directionX *= -1; // Reverse direction
                moveSpeed = chaseSpeed * 0.5f;
            }
        }
        
        // Move horizontally only, preserve vertical velocity (gravity)
        rb.linearVelocity = new Vector2(directionX * moveSpeed, rb.linearVelocity.y);

        // Face the player
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            Flip();
        }

        animator.SetBool("isWalking", true);
    }

    private void PrepareAttack()
    {
        if (player == null) return;

        // Stop completely and face player during preparation
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        animator.SetBool("isWalking", false);

        // Face the player
        float directionX = player.position.x - transform.position.x;
        if ((directionX > 0 && !facingRight) || (directionX < 0 && facingRight))
        {
            Flip();
        }
    }

    private void ExecuteAttack()
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

        // Trigger attack animation and store target
        animator.SetTrigger("isAttacking");
        targetPlayer = player.GetComponent<PlayerStats>();
        
        lastAttackTime = Time.time;
        isFirstAttackInSession = false; // No longer first attack
        
        // Transition to recovery state
        currentState = EnemyState.Recover;
        stateTimer = recoveryDuration;
        isAttacking = false;
    }

    private void RecoverFromAttack()
    {
        if (player == null) return;

        // Back away slightly during recovery
        float directionX = Mathf.Sign(transform.position.x - player.position.x); // Away from player
        float backoffSpeed = chaseSpeed * 0.5f;
        
        rb.linearVelocity = new Vector2(directionX * backoffSpeed, rb.linearVelocity.y);
        
        animator.SetBool("isWalking", true);

        // Keep facing the player even while backing away
        float directionToPlayer = player.position.x - transform.position.x;
        if ((directionToPlayer > 0 && !facingRight) || (directionToPlayer < 0 && facingRight))
        {
            Flip();
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
            // When hit during attack preparation or recovery, interrupt and chase
            if (currentState == EnemyState.PrepareAttack || currentState == EnemyState.Recover)
            {
                currentState = EnemyState.Chase;
                currentSpeed = chaseSpeed;
                isAttacking = false;
            }
            // When hit, switch to chase if not already
            else if (currentState == EnemyState.Patrol)
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
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, optimalAttackDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, losePlayerRange);
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
    public void DealDamage()
    {
        if (targetPlayer == null)
        {
            targetPlayer = PlayerStats.Instance;
        }

        if (targetPlayer != null)
        {
            if (targetPlayer.gameObject.activeInHierarchy)
            {
                targetPlayer.TakeDamage(damage);
            }
        }
    }
}