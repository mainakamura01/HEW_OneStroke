
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField]
    private Vector3 cameraPosition =
        new Vector3(0f, 15f, -12f);

    [SerializeField]
    private Vector3 lookAtPosition =
        Vector3.zero;

    [SerializeField] private float orthographicSize = 12f;

    private Camera cameraComponent;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }

    private void Start()
    {
        transform.position = cameraPosition;
        transform.LookAt(lookAtPosition);

        cameraComponent.orthographic = true;
        cameraComponent.orthographicSize = orthographicSize;
    }
}
