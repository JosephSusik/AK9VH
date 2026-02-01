using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Base Stats")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float attackDamage = 20f;
    public int upgradePoints = 0;

    [Header("Invincibility")]
    public float invincibilityDuration = 1f;
    private float invincibilityTimer;

    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.transform.position = this.transform.position;
            Instance.gameObject.SetActive(true);

            Rigidbody2D targetRb = Instance.GetComponent<Rigidbody2D>();
            if (targetRb != null) targetRb.linearVelocity = Vector2.zero;

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        RegenerateStamina();
        UpdateUI();

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    private void RegenerateStamina()
    {
        if (CurrentStamina < maxStamina)
        {
            CurrentStamina = Mathf.MoveTowards(CurrentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (invincibilityTimer > 0)
        {
            return;
        }

        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);

        if (animator != null)
        {
            animator.SetTrigger("IsHurt");
        }

        invincibilityTimer = invincibilityDuration;

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public bool UseStamina(float amount)
    {
        if (CurrentStamina < amount)
        {
            return false;
        }

        CurrentStamina -= amount;
        return true;
    }

    private void UpdateUI()
    {
        if (healthFill != null)
        {
            healthFill.fillAmount = CurrentHealth / maxHealth;
        }
        if (staminaFill != null)
        {
            staminaFill.fillAmount = CurrentStamina / maxStamina;
        }
    }

    private void Die()
    {
        SceneManager.Instance.ChangeScene(SceneManager.GameScene.GameOver);
    }

    public void AddUpgradePoints(int amount)
    {
        upgradePoints += amount;
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        bool isGameplayScene = (scene.name == "Level1" || scene.name == "Level2" || scene.name == "Upgrade");
        gameObject.SetActive(isGameplayScene);
        
        if (!isGameplayScene)
        {
            return;
        }

        GameObject healthObj = GameObject.Find("HealthBarFill");
        GameObject staminaObj = GameObject.Find("StaminaBarFill");

        if (healthObj != null)
        {
            healthFill = healthObj.GetComponent<Image>();
        }
        if (staminaObj != null)
        {
            staminaFill = staminaObj.GetComponent<Image>();
        }

        if (scene.name == "Level1")
        {
            CurrentHealth = 100;
            CurrentStamina = 100;
            attackDamage = 20f;
            invincibilityTimer = 0;
            upgradePoints = 0;
            if (animator != null)
            {
                animator.Play("Idle");
            }
        }

        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
            transform.position = spawnPoint.transform.position;
        }

        CinemachineCamera cmCam = FindFirstObjectByType<CinemachineCamera>();
        if (cmCam != null)
        {
            cmCam.Follow = transform;
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerInput.enabled = true;
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }
}