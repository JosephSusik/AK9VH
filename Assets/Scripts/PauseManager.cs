using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuObject;
    [SerializeField] private TextMeshProUGUI statsDisplayText;

    private PlayerStats pStats;
    private bool isPaused = false;

    public void OnPause(InputValue value)
    {
        if (value.isPressed) TogglePause();
    }

    public void OnMenu(InputValue value)
    {
        if (value.isPressed) BackToMenu();
    }

    public void OnQuit(InputValue value)
    {
        if (value.isPressed)
        {
            Debug.Log("Exiting Application...");
            Application.Quit();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuObject.SetActive(isPaused);

        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if (pStats == null) pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

        statsDisplayText.text = $"Health: {Mathf.RoundToInt(pStats.currentHealth)} / {pStats.maxHealth}\n" +
                                $"Stamina: {Mathf.RoundToInt(pStats.currentStamina)} / {pStats.maxStamina}\n" +
                                $"Atk power: {pStats.attackDamage}";
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}