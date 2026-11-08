
using UnityEngine;

public class CharacterIdleMotion : MonoBehaviour
{
    [Header("Idle Motion")]
    [SerializeField] private float bobHeight = 0.06f;
    [SerializeField] private float bobSpeed = 2.5f;

    private Vector3 startLocalPosition;

    private void Start()
    {
        startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float offsetY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;

        transform.localPosition =
            startLocalPosition + Vector3.up * offsetY;
    }
}
