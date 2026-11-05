
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image damageOverlay;

    [Header("Flash Settings")]
    [SerializeField] private float flashDuration = 0.3f;

    [Range(0f, 1f)]
    [SerializeField] private float maxAlpha = 0.35f;

    private Coroutine flashCoroutine;

    private void Awake()
    {
        SetAlpha(0f);
    }

    public void PlayFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }

        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float elapsed = 0f;

        SetAlpha(maxAlpha);

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, flashDuration)
            );

            SetAlpha(Mathf.Lerp(maxAlpha, 0f, t));

            yield return null;
        }

        SetAlpha(0f);
        flashCoroutine = null;
    }

    private void SetAlpha(float alpha)
    {
        if (damageOverlay == null)
            return;

        Color color = damageOverlay.color;
        color.a = alpha;
        damageOverlay.color = color;
    }
}
