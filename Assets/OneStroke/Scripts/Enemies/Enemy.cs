
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHP = 3;

    [Header("Defeat Effect")]
    [SerializeField] private SpriteRenderer enemySprite;
    [SerializeField] private Color hitColor = Color.red;
    [SerializeField] private float defeatDuration = 0.2f;

    private int currentHP;
    private bool isDead = false;

    private ScoreManager scoreManager;

    private EnemyDeathEffect enemyDeathEffect;

    private void Awake()
    {
        currentHP = maxHP;

        if (enemySprite == null)
        {
            enemySprite = GetComponentInChildren<SpriteRenderer>();
        }
    }

    private void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();

        enemyDeathEffect =
            FindFirstObjectByType<EnemyDeathEffect>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHP -= damage;

        Debug.Log(
            $"{gameObject.name} took {damage} damage! HP: {currentHP}"
        );

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        Debug.Log($"{gameObject.name} defeated!");

        if (scoreManager != null)
        {
            scoreManager.AddEnemyScore();
        }
        else
        {
            Debug.LogWarning(
                "Enemy: ScoreManager was not found!"
            );
        }

        // Œ‚”j‚³‚ê‚½uŠÔ‚É—±Žq‚ð”ò‚Î‚·
        if (enemyDeathEffect != null)
        {
            enemyDeathEffect.PlayEffect(transform.position);
        }

        StartCoroutine(DefeatEffect());
    }

    private IEnumerator DefeatEffect()
    {
        if (enemySprite == null)
        {
            Destroy(gameObject);
            yield break;
        }

        // Œ‚”j‚³‚ê‚½uŠÔ‚ÉÔ‚­‚·‚é
        enemySprite.color = hitColor;

        Vector3 originalScale = enemySprite.transform.localScale;
        Color startColor = enemySprite.color;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, defeatDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            // ™X‚É¬‚³‚­‚·‚é
            enemySprite.transform.localScale =
                Vector3.Lerp(originalScale, Vector3.zero, t);

            // ™X‚É“§–¾‚É‚·‚é
            Color color = startColor;
            color.a = Mathf.Lerp(startColor.a, 0f, t);
            enemySprite.color = color;

            yield return null;
        }

        Destroy(gameObject);
    }
}
