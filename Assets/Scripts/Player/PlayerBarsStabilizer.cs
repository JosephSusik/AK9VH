using UnityEngine;

public class PlayerBarsStabilizer : MonoBehaviour
{
    private Vector3 localScale;

    void Start()
    {
        localScale = transform.localScale;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.identity;
        Vector3 parentScale = transform.parent.localScale;

        transform.localScale = new Vector3(
            localScale.x / Mathf.Sign(parentScale.x),
            localScale.y / Mathf.Sign(parentScale.y),
            localScale.z
        );
    }
}