
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private Image waveBackground;

    [Header("Colors")]
    [SerializeField]
    private Color normalColor =
        new Color(0.15f, 0.35f, 0.5f, 1f);

    [SerializeField]
    private Color enhancedColor =
        new Color(0.65f, 0.2f, 0.2f, 1f);

    private void Start()
    {
        UpdateWaveUI();
    }

    private void Update()
    {
        UpdateWaveUI();
    }

    private void UpdateWaveUI()
    {
        if (enemySpawner == null)
            return;

        bool isEnhanced = enemySpawner.IsEnhancedWave;

        if (waveText != null)
        {
            waveText.text = isEnhanced
                ? "ENHANCED WAVE"
                : "NORMAL WAVE";
        }

        if (waveBackground != null)
        {
            waveBackground.color = isEnhanced
                ? enhancedColor
                : normalColor;
        }
    }
}
