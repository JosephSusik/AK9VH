using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuObject;

    private bool isPaused = false;

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            pauseMenuObject.SetActive(true);
            Time.timeScale = 0f;
            GetComponent<PlayerInput>().actions.FindActionMap("Player").Disable();
        }
        else
        {
            pauseMenuObject.SetActive(false);
            Time.timeScale = 1f;
            GetComponent<PlayerInput>().actions.FindActionMap("Player").Enable();
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