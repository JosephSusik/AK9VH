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

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenuObject.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;

        if (isPaused)
        {
            if (pStats == null) pStats = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerStats>();

            statsDisplayText.text = $"Health: {Mathf.RoundToInt(pStats.currentHealth)} / {pStats.maxHealth}\n" +
                                    $"Stamina: {Mathf.RoundToInt(pStats.currentStamina)} / {pStats.maxStamina}\n" +
                                    $"Atk Power: {pStats.attackDamage}";
        }
    }

    public void OnMenu(InputValue value)
    {
        if (isPaused && value.isPressed)
        {
            BackToMenu();
        }
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}