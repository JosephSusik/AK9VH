using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;
    public void OnStartButtonPressed() => levelManager.LoadGame();

    public void OnControlsButtonPressed() => levelManager.LoadControls();

    public void OnBackButtonPressed() => levelManager.LoadMenu();

    public void OnQuitButtonPressed() => levelManager.QuitGame();
}