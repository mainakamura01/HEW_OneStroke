
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera targetCamera;

    private void LateUpdate()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        if (targetCamera == null)
            return;

        transform.rotation = targetCamera.transform.rotation;
    }
}
