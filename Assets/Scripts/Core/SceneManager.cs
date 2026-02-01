using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public static SceneManager Instance { get; private set; }

    public enum GameScene
    {
        MainMenu,
        Controls,
        Level1,
        Level2,
        Upgrade,
        GameOver
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ChangeScene(GameScene scene)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene.ToString());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        ChangeScene(GameScene.Level1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}