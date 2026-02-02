using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseController : BaseController
{
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private TextMeshProUGUI statsText;
    private bool isPaused;  

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
        {
            TogglePause();
        }
    }

    public override void BackToMenu()
    {
        Time.timeScale = 1f;
        base.BackToMenu();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuObject.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            UpdatePausedText();
        }
    }

    private void UpdatePausedText()
    {
        var stats = PlayerStats.Instance;
        statsText.text = $"Health: {Mathf.RoundToInt(stats.CurrentHealth)} / {stats.maxHealth}\n" +
                         $"Stamina: {Mathf.RoundToInt(stats.CurrentStamina)} / {stats.maxStamina}\n" +
                         $"Attack power: {stats.attackDamage}\n" +
                         $"Upgrade points: {stats.upgradePoints}";
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        GameObject foundUI = GameObject.Find("PauseOverlay");

        if (foundUI != null)
        {
            pauseMenuObject = foundUI;
            statsText = foundUI.GetComponentInChildren<TextMeshProUGUI>();
            pauseMenuObject.SetActive(false);
        }
    }
}