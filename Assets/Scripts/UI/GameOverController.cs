using UnityEngine;
using UnityEngine.InputSystem;

public class GameOverController : MonoBehaviour
{
    public void OnRestart(InputValue value)
    {
        if (value.isPressed)
        {
            RestartLevel();
        }
    }

    public void OnMenu(InputValue value)
    {
        if (value.isPressed)
        {
            BackToMenu();
        }
    }

    public void OnQuit(InputValue value)
    {
        if (value.isPressed)
        {
            QuitGame();
        }
    }

    public void RestartLevel() => LevelManager.Instance.LoadGame();
    public void BackToMenu() => LevelManager.Instance.LoadMenu();
    public void QuitGame() => LevelManager.Instance.QuitGame();
}