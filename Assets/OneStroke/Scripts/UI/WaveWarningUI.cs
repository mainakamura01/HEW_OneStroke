
using System.Collections;
using UnityEngine;

public class WaveWarningUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private CanvasGroup warningPanelGroup;
    [SerializeField] private CanvasGroup warningBannerGroup;

    [Header("Animation Settings")]
    [SerializeField] private float fadeInDuration = 0.2f;
    [SerializeField] private float blinkDuration = 0.15f;
    [SerializeField] private int blinkCount = 3;
    [SerializeField] private float holdDuration = 0.5f;
    [SerializeField] private float fadeOutDuration = 0.4f;

    private bool previousEnhancedWave;
    private Coroutine warningCoroutine;

    private void Start()
    {
        if (warningPanelGroup != null)
        {
            warningPanelGroup.alpha = 0f;
            warningPanelGroup.interactable = false;
            warningPanelGroup.blocksRaycasts = false;
        }

        if (warningBannerGroup != null)
        {
            warningBannerGroup.alpha = 1f;
        }

        if (enemySpawner != null)
        {
            previousEnhancedWave =
                enemySpawner.IsEnhancedWave;
        }
    }

    private void Update()
    {
        if (enemySpawner == null)
            return;

        bool currentEnhancedWave =
            enemySpawner.IsEnhancedWave;

        if (currentEnhancedWave && !previousEnhancedWave)
        {
            PlayWarning();
        }

        previousEnhancedWave = currentEnhancedWave;
    }

    private void PlayWarning()
    {
        if (warningPanelGroup == null ||
            warningBannerGroup == null)
        {
            Debug.LogWarning(
                "WaveWarningUI: CanvasGroup is missing!"
            );
            return;
        }

        if (warningCoroutine != null)
        {
            StopCoroutine(warningCoroutine);
        }

        warningCoroutine =
            StartCoroutine(WarningRoutine());
    }

    private IEnumerator WarningRoutine()
    {
        warningBannerGroup.alpha = 1f;

        // 全体をフェードイン
        yield return Fade(
            warningPanelGroup,
            0f,
            1f,
            fadeInDuration
        );

        // 警告帯だけ点滅
        for (int i = 0; i < blinkCount; i++)
        {
            yield return Fade(
                warningBannerGroup,
                1f,
                0.2f,
                blinkDuration
            );

            yield return Fade(
                warningBannerGroup,
                0.2f,
                1f,
                blinkDuration
            );
        }

        // 少し表示したままにする
        yield return new WaitForSecondsRealtime(
            holdDuration
        );

        // 全体をフェードアウト
        yield return Fade(
            warningPanelGroup,
            1f,
            0f,
            fadeOutDuration
        );

        warningBannerGroup.alpha = 1f;
        warningCoroutine = null;
    }

    private IEnumerator Fade(
        CanvasGroup group,
        float from,
        float to,
        float duration)
    {
        if (duration <= 0f)
        {
            group.alpha = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / duration
            );

            group.alpha = Mathf.Lerp(
                from,
                to,
                t
            );

            yield return null;
        }

        group.alpha = to;
    }
}
