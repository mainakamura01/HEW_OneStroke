
using UnityEngine;

public class ShadowRotationLock : MonoBehaviour
{
    [Header("Shadow Position")]
    [SerializeField] private float groundOffset = 0.03f;
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetZ = -1.1f;

    [Header("Shadow Rotation")]
    [SerializeField]
    private Vector3 worldRotation =
        new Vector3(90f, 0f, 0f);

    private void LateUpdate()
    {
        if (transform.parent == null)
            return;

        Vector3 playerPosition = transform.parent.position;

        transform.position = new Vector3(
            playerPosition.x + offsetX,
            groundOffset,
            playerPosition.z + offsetZ
        );

        transform.rotation = Quaternion.Euler(worldRotation);
    }
}
