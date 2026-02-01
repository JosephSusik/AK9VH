using UnityEngine;
using TMPro;

public class UpgradeController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI availablePointsText;
    [SerializeField] private TextMeshProUGUI healthStatsText;
    [SerializeField] private TextMeshProUGUI staminaStatsText;
    [SerializeField] private TextMeshProUGUI attackStatsText;

    [Header("Settings")]
    public int upgradeCost = 1;

    private void Update()
    {
        if (PlayerStats.Instance != null)
        {
            availablePointsText.text = $"Points: {PlayerStats.Instance.upgradePoints}";
            healthStatsText.text = $"Max health: {PlayerStats.Instance.maxHealth}";
            staminaStatsText.text = $"Max stamina: {PlayerStats.Instance.maxStamina}";
            attackStatsText.text = $"Attack power: {PlayerStats.Instance.attackDamage}";
        }
    }

    public void UpgradeHealth()
    {
        if (PlayerStats.Instance.upgradePoints >= upgradeCost)
        {
            PlayerStats.Instance.upgradePoints -= upgradeCost;
            PlayerStats.Instance.maxHealth += 20f;
            // The least intuitive way to heal the player
            PlayerStats.Instance.TakeDamage(-20f);
        }
    }

    public void UpgradeStamina()
    {
        if (PlayerStats.Instance.upgradePoints >= upgradeCost)
        {
            PlayerStats.Instance.upgradePoints -= upgradeCost;
            PlayerStats.Instance.maxStamina += 20f;
        }
    }

    public void UpgradeAttack()
    {
        if (PlayerStats.Instance.upgradePoints >= upgradeCost)
        {
            PlayerStats.Instance.upgradePoints -= upgradeCost;
            PlayerStats.Instance.attackDamage += 5f;
        }
    }

    public void StartNextLevel()
    {
        LevelManager.Instance.LoadLevelByName("Level2");
    }

    public void BackToMenu()
    {
        LevelManager.Instance.LoadMenu();
    }
}