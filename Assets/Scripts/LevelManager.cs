using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // We use string variables so you can change scene names in the Inspector
    [SerializeField] private string startLevelName = "Level1";
    [SerializeField] private string controlsLevelName = "Controls";
    [SerializeField] private string mainMenuName = "MainMenu";

    public void LoadGame() => SceneManager.LoadScene(startLevelName);

    public void LoadControls() => SceneManager.LoadScene(controlsLevelName);

    public void LoadMenu() => SceneManager.LoadScene(mainMenuName);

    public void QuitGame()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}