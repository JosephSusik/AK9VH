using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseController : MonoBehaviour
{
    public virtual void OnMenu(InputValue value)
    {
        if (value.isPressed) BackToMenu();
    }

    public virtual void OnRestart(InputValue value)
    {
        if (value.isPressed) RestartGame();
    }

    public virtual void OnQuit(InputValue value)
    {
        if (value.isPressed) QuitGame();
    }

    public virtual void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneManager.GameScene.MainMenu);
    }

    public virtual void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.RestartGame();
    }

    public virtual void QuitGame() => SceneManager.Instance.QuitGame();
}