
using UnityEngine;

public class SmoothFollowCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Camera Position")]
    [SerializeField]
    private Vector3 offset =
        new Vector3(0f, 14f, -12f);

    [Header("Follow Settings")]
    [SerializeField] private float smoothTime = 0.15f;

    [Header("Dead Zone Settings")]
    [SerializeField] private float deadZoneWidth = 3f;
    [SerializeField] private float deadZoneDepth = 3f;

    [Header("Look Ahead Settings")]
    [SerializeField] private float lookAheadDistance = 0f;
    [SerializeField] private float lookAheadSmoothTime = 0.5f;

    [Header("Look At Settings")]
    [SerializeField] private float lookAtHeight = 1f;

    private Vector3 followCenter;
    private Vector3 followVelocity;

    private Vector3 lookAheadOffset;
    private Vector3 lookAheadVelocity;
    private Vector3 previousTargetPosition;

    private Quaternion fixedRotation;

    private void Start()
    {
        if (target == null)
            return;

        followCenter = target.position;
        previousTargetPosition = target.position;

        // ƒJƒƒ‰‚ÌŒü‚«‚ðÅ‰‚ÉŒˆ‚ß‚ÄŒÅ’è‚·‚é
        Vector3 lookDirection =
            -offset + Vector3.up * lookAtHeight;

        fixedRotation = Quaternion.LookRotation(
            lookDirection,
            Vector3.up
        );

        transform.position = followCenter + offset;
        transform.rotation = fixedRotation;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Vector3 playerPosition = target.position;

        // ‡@ ƒvƒŒƒCƒ„[‚ÌˆÚ“®•ûŒü‚ðŽæ“¾
        Vector3 movement =
            playerPosition - previousTargetPosition;

        movement.y = 0f;

        Vector3 desiredLookAhead = Vector3.zero;

        if (movement.sqrMagnitude > 0.000001f)
        {
            desiredLookAhead =
                movement.normalized * lookAheadDistance;
        }

        lookAheadOffset = Vector3.SmoothDamp(
            lookAheadOffset,
            desiredLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );

        // ‡A ƒfƒbƒhƒ][ƒ“‚ÌŠO‚Éo‚½•ª‚¾‚¯’Ç]’†S‚ð“®‚©‚·
        float halfWidth = Mathf.Max(0f, deadZoneWidth) * 0.5f;
        float halfDepth = Mathf.Max(0f, deadZoneDepth) * 0.5f;

        float differenceX =
            playerPosition.x - followCenter.x;

        if (differenceX > halfWidth)
        {
            followCenter.x =
                playerPosition.x - halfWidth;
        }
        else if (differenceX < -halfWidth)
        {
            followCenter.x =
                playerPosition.x + halfWidth;
        }

        float differenceZ =
            playerPosition.z - followCenter.z;

        if (differenceZ > halfDepth)
        {
            followCenter.z =
                playerPosition.z - halfDepth;
        }
        else if (differenceZ < -halfDepth)
        {
            followCenter.z =
                playerPosition.z + halfDepth;
        }

        // ‡B ƒJƒƒ‰‚ð‚È‚ß‚ç‚©‚É’Ç]‚³‚¹‚é
        Vector3 desiredPosition =
            followCenter + offset + lookAheadOffset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            smoothTime
        );

        // ‡C ƒJƒƒ‰‚ÌŠp“x‚ÍŒÅ’è
        transform.rotation = fixedRotation;

        previousTargetPosition = playerPosition;
    }
}
