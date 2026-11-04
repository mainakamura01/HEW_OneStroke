
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;

    private void Start()
    {
        UpdateHealthUI();
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        int currentHP = playerHealth.CurrentHP;
        int maxHP = playerHealth.MaxHP;

        if (hpFill != null)
        {
            float hpRatio =
            maxHP > 0 ? (float)currentHP / maxHP : 0f;

            hpFill.rectTransform.SetSizeWithCurrentAnchors(
                RectTransform.Axis.Horizontal,
                300f * hpRatio
            );
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHP} / {maxHP}";
        }
    }
}
