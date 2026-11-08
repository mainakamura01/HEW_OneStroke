
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeGlowRing : MonoBehaviour
{
    [Header("Ring Settings")]
    [SerializeField] private float radius = 1.15f;
    [SerializeField] private int segments = 64;

    [Header("Pulse Settings")]
    [SerializeField] private float pulseSpeed = 2.5f;
    [SerializeField] private float pulseAmount = 0.12f;
    [SerializeField] private float fadeSpeed = 6f;

    private LineRenderer lineRenderer;

    private bool targetVisible = false;
    private float visibility = 0f;
    private float pulseTime = 0f;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;

        for (int i = 0; i < segments; i++)
        {
            float angle =
                i * Mathf.PI * 2f / segments;

            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            lineRenderer.SetPosition(
                i,
                new Vector3(x, 0.03f, z)
            );
        }

        lineRenderer.enabled = false;
        transform.localScale = Vector3.one;
    }

    private void Update()
    {
        // 表示・非表示を滑らかに切り替える
        float target = targetVisible ? 1f : 0f;

        visibility = Mathf.MoveTowards(
            visibility,
            target,
            fadeSpeed * Time.deltaTime
        );

        if (visibility <= 0f)
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;

        // リングをゆっくり脈打たせる
        pulseTime += Time.deltaTime * pulseSpeed;

        float pulse =
            (Mathf.Sin(pulseTime) + 1f) * 0.5f;

        float pulseScale =
            1f + pulse * pulseAmount;

        // 出現時は小さく、徐々に広がる
        float appearScale =
            Mathf.Lerp(0.85f, 1f, visibility);

        transform.localScale =
            Vector3.one * pulseScale * appearScale;

        // リングの透明度を滑らかに変更
        Color color = new Color(
            0.4f,
            0.95f,
            1f,
            visibility
        );

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetVisible(bool visible)
    {
        targetVisible = visible;
    }
}
