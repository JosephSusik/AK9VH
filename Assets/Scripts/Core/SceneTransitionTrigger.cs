using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private SceneManager.GameScene targetScene;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.Instance.ChangeScene(targetScene);
        }
    }
}