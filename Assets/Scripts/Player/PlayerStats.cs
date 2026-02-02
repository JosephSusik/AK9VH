using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Default Stats")]
    [SerializeField] private float defaultMaxHealth = 100f;
    [SerializeField] private float defaultMaxStamina = 100f;
    [SerializeField] private float defaultAttackDamage = 20f;

    [Header("Base Stats")]
    public float maxHealth;
    public float maxStamina;
    public float attackDamage;
    public float staminaRegenRate = 15f;
    public int upgradePoints = 0;

    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }

    private Animator animator;
    private Rigidbody2D rb;
    private PlayerInput playerInput;
    private float invincibilityTimer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Instance.gameObject.SetActive(true);
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        StatsInit();
    }

    public void StatsInit()
    {
        maxHealth = defaultMaxHealth;
        maxStamina = defaultMaxStamina;
        attackDamage = defaultAttackDamage;

        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
        upgradePoints = 0;
        invincibilityTimer = 0;

        if (animator != null)
        {
            animator.Play("Idle");
        }

        UpdateUI();
    }

    private void Update()
    {
        if (CurrentStamina < maxStamina)
        {
            CurrentStamina = Mathf.MoveTowards(CurrentStamina, maxStamina, staminaRegenRate * Time.deltaTime);
        }

        if (invincibilityTimer > 0)
        {
            invincibilityTimer -= Time.deltaTime;
        }
        UpdateUI();
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

        invincibilityTimer = 1f;

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

        if (scene.name == "Level1")
        {
            StatsInit();
        }

        if (playerInput != null)
        {
            playerInput.enabled = false;
            playerInput.enabled = true;
            playerInput.SwitchCurrentActionMap("Gameplay");
        }
    }
}