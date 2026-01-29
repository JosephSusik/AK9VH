using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour
{
    [SerializeField] private LevelManager levelManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            levelManager.LoadGame();
        }
    }
}
