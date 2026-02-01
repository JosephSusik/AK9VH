using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
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
        LevelManager.Instance.LoadMenu();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        LevelManager.Instance.LoadGame();
    }

    public void QuitGame()
    {
        LevelManager.Instance.QuitGame();
    }

    private void UpdatePausedText()
    {
        statsText.text = $"Health: {Mathf.RoundToInt(PlayerStats.Instance.CurrentHealth)} / {PlayerStats.Instance.maxHealth}\n" +
                         $"Stamina: {Mathf.RoundToInt(PlayerStats.Instance.CurrentStamina)} / {PlayerStats.Instance.maxStamina}\n" +
                         $"Attack: {PlayerStats.Instance.attackDamage}\n" +
                         $"Upgrade points: {PlayerStats.Instance.upgradePoints}";
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
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