using UnityEngine;
using Unity.Cinemachine;

public class PlayerSpawner : MonoBehaviour
{
    private void Start()
    {
        if (PlayerStats.Instance != null)
        {
            GameObject player = PlayerStats.Instance.gameObject;
            player.transform.position = transform.position;

            if (player.TryGetComponent(out Rigidbody2D rb))
            {
                rb.linearVelocity = Vector2.zero;
            }

            CinemachineCamera cmCam = FindFirstObjectByType<CinemachineCamera>();
            if (cmCam != null)
            {
                cmCam.Follow = player.transform;
            }
        }
    }
}