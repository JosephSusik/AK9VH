using UnityEngine;

public class ChangeLevelTrigger : MonoBehaviour
{
    [SerializeField] private string levelToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            LevelManager.Instance.LoadLevelByName(levelToLoad);
        }
    }
}