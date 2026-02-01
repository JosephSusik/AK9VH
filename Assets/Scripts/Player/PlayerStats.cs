using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Settings")]
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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        animator = GetComponent<Animator>();
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
        animator.SetTrigger("IsHurt");

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
        healthFill.fillAmount = CurrentHealth / maxHealth;
        staminaFill.fillAmount = CurrentStamina / maxStamina;
    }

    private void Die()
    {
        LevelManager.Instance.LoadGameOver();
    }

    public void AddUpgradePoints(int amount)
    {
        upgradePoints += amount;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindWithTag("SpawnPoint");
        if (spawnPoint != null)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                transform.position = spawnPoint.transform.position;
            }
        }

        CinemachineCamera cmCam = FindFirstObjectByType<CinemachineCamera>();
        if (cmCam != null)
        {
            cmCam.Follow = transform;
        }

        PlayerInput input = GetComponent<PlayerInput>();
        if (input != null)
        {
            input.enabled = false;
            input.enabled = true;
            input.SwitchCurrentActionMap("Gameplay");
        }
    }
}