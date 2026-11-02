
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 12f;

    private CharacterController controller;
    private Vector3 moveDirection;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }


    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 inputDirection =
            new Vector3(horizontal, 0f, vertical);

        inputDirection = Vector3.ClampMagnitude(
            inputDirection, 1f
        );

        Transform cameraTransform = Camera.main.transform;

        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection =
            cameraForward * inputDirection.z +
            cameraRight * inputDirection.x;

        moveDirection = Vector3.ClampMagnitude(
            moveDirection, 1f
        );

        bool isDashing = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isDashing ? dashSpeed : moveSpeed;

        controller.Move(
            moveDirection * currentSpeed * Time.deltaTime
        );
    }


    private void HandleRotation()
    {
        if (moveDirection.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(moveDirection, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
