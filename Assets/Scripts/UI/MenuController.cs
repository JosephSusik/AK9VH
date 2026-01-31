using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void OnStartButtonPressed() => LevelManager.Instance.LoadGame();

    public void OnControlsButtonPressed() => LevelManager.Instance.LoadControls();

    public void OnBackButtonPressed() => LevelManager.Instance.LoadMenu();

    public void OnQuitButtonPressed() => LevelManager.Instance.QuitGame();
}