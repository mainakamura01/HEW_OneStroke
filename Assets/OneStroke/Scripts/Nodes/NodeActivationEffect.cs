
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NodeActivationEffect : MonoBehaviour
{
    [Header("Ring Settings")]
    [SerializeField] private float startRadius = 0.45f;
    [SerializeField] private float endRadius = 1.65f;
    [SerializeField] private int segments = 64;

    [Header("Animation Settings")]
    [SerializeField] private float duration = 0.35f;
    [SerializeField] private float startWidth = 0.16f;

    private LineRenderer lineRenderer;
    private Coroutine effectCoroutine;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.useWorldSpace = false;
        lineRenderer.loop = true;
        lineRenderer.positionCount = segments;

        lineRenderer.enabled = false;
    }

    public void PlayEffect()
    {
        if (effectCoroutine != null)
        {
            StopCoroutine(effectCoroutine);
        }

        effectCoroutine = StartCoroutine(EffectRoutine());
    }

    private IEnumerator EffectRoutine()
    {
        lineRenderer.enabled = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, duration)
            );

            // Å‰‚Í‘¬‚­AÅŒã‚Í‚ä‚Á‚­‚èL‚ª‚é
            float easedT = 1f - Mathf.Pow(1f - t, 3f);

            float radius = Mathf.Lerp(
                startRadius,
                endRadius,
                easedT
            );

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

            // ™X‚Éü‚ð×‚­‚·‚é
            float width = Mathf.Lerp(
                startWidth,
                0f,
                t
            );

            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;

            // ™X‚É“§–¾‚É‚·‚é
            Color color = new Color(
                1f,
                0.39f,
                0.53f,
                1f - t
            );

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            yield return null;
        }

        lineRenderer.enabled = false;
        effectCoroutine = null;
    }
}
