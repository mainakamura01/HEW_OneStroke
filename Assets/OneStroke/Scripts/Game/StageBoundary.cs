
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class StageBoundary : MonoBehaviour
{
    [Header("Stage Settings")]
    [SerializeField] private float stageWidth = 60f;
    [SerializeField] private float stageDepth = 60f;
    [SerializeField] private float lineHeight = 0.15f;

    [Header("Line Settings")]
    [SerializeField] private float lineWidth = 0.25f;
    [SerializeField] private Color lineColor = Color.cyan;

    private LineRenderer lineRenderer;

    public float HalfWidth => stageWidth * 0.5f;
    public float HalfDepth => stageDepth * 0.5f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupBoundary();
    }

    private void SetupBoundary()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = 4;

        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;

        float x = HalfWidth;
        float z = HalfDepth;

        lineRenderer.SetPosition(
            0, new Vector3(-x, lineHeight, -z)
        );

        lineRenderer.SetPosition(
            1, new Vector3(-x, lineHeight, z)
        );

        lineRenderer.SetPosition(
            2, new Vector3(x, lineHeight, z)
        );

        lineRenderer.SetPosition(
            3, new Vector3(x, lineHeight, -z)
        );
    }

    public Vector3 ClampPosition(Vector3 position)
    {
        Vector3 local = transform.InverseTransformPoint(position);

        local.x = Mathf.Clamp(
            local.x, -HalfWidth, HalfWidth
        );

        local.z = Mathf.Clamp(
            local.z, -HalfDepth, HalfDepth
        );

        return transform.TransformPoint(local);
    }
}
