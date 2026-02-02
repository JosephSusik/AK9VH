using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void OnStartButtonPressed() => SceneManager.Instance.ChangeScene(SceneManager.GameScene.Level1);

    public void OnControlsButtonPressed() => SceneManager.Instance.ChangeScene(SceneManager.GameScene.Controls);

    public void OnBackButtonPressed() => SceneManager.Instance.ChangeScene(SceneManager.GameScene.MainMenu);

    public void OnQuitButtonPressed() => SceneManager.Instance.QuitGame();

    public void OnRestartButtonPressed() => SceneManager.Instance.RestartGame();
}