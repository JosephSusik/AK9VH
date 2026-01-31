using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image healthFill;
    [SerializeField] private Image staminaFill;

    [Header("Settings")]
    public float maxHealth = 100f;
    public float maxStamina = 100f;
    public float staminaRegenRate = 15f;
    public float attackDamage = 20f;

    public float CurrentHealth { get; private set; }
    public float CurrentStamina { get; private set; }

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        CurrentHealth = maxHealth;
        CurrentStamina = maxStamina;
    }

    private void Update()
    {
        RegenerateStamina();
        UpdateUI();
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
        CurrentHealth = Mathf.Clamp(CurrentHealth - damage, 0, maxHealth);
        animator.SetTrigger("IsHurt");
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
}