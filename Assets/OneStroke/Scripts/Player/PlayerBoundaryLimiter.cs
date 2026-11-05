
using UnityEngine;

public class PlayerBoundaryLimiter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageBoundary stageBoundary;

    [Header("Settings")]
    [SerializeField] private float boundaryMargin = 0.6f;

    private void LateUpdate()
    {
        if (stageBoundary == null)
            return;

        Vector3 position = transform.position;

        Vector3 clamped =
            stageBoundary.ClampPosition(position);

        // ÉvÉåÉCÉÑÅ[ÇÃîºåaÇ‘ÇÒì‡ë§Ç…êßå¿
        float margin = Mathf.Max(0f, boundaryMargin);

        Vector3 local =
            stageBoundary.transform.InverseTransformPoint(clamped);

        local.x = Mathf.Clamp(
            local.x,
            -stageBoundary.HalfWidth + margin,
            stageBoundary.HalfWidth - margin
        );

        local.z = Mathf.Clamp(
            local.z,
            -stageBoundary.HalfDepth + margin,
            stageBoundary.HalfDepth - margin
        );

        transform.position =
            stageBoundary.transform.TransformPoint(local);
    }
}
