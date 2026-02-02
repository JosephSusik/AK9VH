using UnityEngine;
using TMPro;

public class UpgradeController : BaseController
{
    [SerializeField] private TextMeshProUGUI availablePointsText;
    [SerializeField] private TextMeshProUGUI healthStatsText;
    [SerializeField] private TextMeshProUGUI staminaStatsText;
    [SerializeField] private TextMeshProUGUI attackStatsText;
    public int upgradeCost = 1;

    private void Update()
    {
        var stats = PlayerStats.Instance;
        if (stats != null)
        {
            availablePointsText.text = $"Points: {stats.upgradePoints}";
            healthStatsText.text = $"Max health: {stats.maxHealth}";
            staminaStatsText.text = $"Max stamina: {stats.maxStamina}";
            attackStatsText.text = $"Attack power: {stats.attackDamage}";
        }
    }

    public void UpgradeHealth() => PurchaseUpgrade(() => {
        PlayerStats.Instance.maxHealth += 20f;
        PlayerStats.Instance.TakeDamage(-20f);
    });

    public void UpgradeStamina() => PurchaseUpgrade(() =>
    {
        PlayerStats.Instance.maxStamina += 20f;
    });

    public void UpgradeAttack() => PurchaseUpgrade(() =>
    {
        PlayerStats.Instance.attackDamage += 5f;
    });

    private void PurchaseUpgrade(System.Action upgrade)
    {
        if (PlayerStats.Instance.upgradePoints >= upgradeCost)
        {
            PlayerStats.Instance.upgradePoints -= upgradeCost;
            upgrade.Invoke();
        }
    }

    public void StartNextLevel()
    {
        SceneManager.Instance.ChangeScene(SceneManager.GameScene.Level2);
    }
}