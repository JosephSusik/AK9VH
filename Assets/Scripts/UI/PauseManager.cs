using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private TextMeshProUGUI statsText;

    private PlayerStats stats;
    private bool isPaused;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

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
            LevelManager.Instance.LoadMenu();
        }
    }

    public void OnQuit(InputValue value)
    {
        if (value.isPressed)
        {
            LevelManager.Instance.QuitGame();
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

    private void UpdatePausedText()
    {
        statsText.text = $"Health: {Mathf.RoundToInt(stats.CurrentHealth)} / {stats.maxHealth}\n" +
                         $"Stamina: {Mathf.RoundToInt(stats.CurrentStamina)} / {stats.maxStamina}\n" +
                         $"Attack: {stats.attackDamage}";
    }
}