using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private string startLevelName = "Level1";
    [SerializeField] private string controlsLevelName = "Controls";
    [SerializeField] private string mainMenuName = "MainMenu";

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(gameObject); return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep alive between levels
    }

    public void LoadGame() => SceneManager.LoadScene(startLevelName);
    public void LoadControls() => SceneManager.LoadScene(controlsLevelName);
    public void LoadMenu() => SceneManager.LoadScene(mainMenuName);
    public void QuitGame() => Application.Quit();
}