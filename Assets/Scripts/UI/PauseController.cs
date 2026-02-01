using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
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

    public void OnMenu(InputValue value)
    {
        if (value.isPressed)
        {
            BackToMenu();
        }
    }

    public void OnRestart(InputValue value)
    {
        if (value.isPressed)
        {
            RestartGame();
        }
    }

    public void OnQuit(InputValue value)
    {
        if (value.isPressed)
        {
            QuitGame();
        }
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

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneManager.GameScene.MainMenu);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.RestartGame();
    }

    public void QuitGame()
    {
        SceneManager.Instance.QuitGame();
    }

    private void UpdatePausedText()
    {
        var stats = PlayerStats.Instance;
        statsText.text = $"Health: {Mathf.RoundToInt(stats.CurrentHealth)} / {stats.maxHealth}\n" +
                         $"Stamina: {Mathf.RoundToInt(stats.CurrentStamina)} / {stats.maxStamina}\n" +
                         $"Attack: {stats.attackDamage}\n" +
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
            statsText = foundUI.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            pauseMenuObject.SetActive(false);
        }
    }
}