
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private int maxHP = 3;

    private int currentHP;
    private bool isDead = false;

    private ScoreManager scoreManager;

    private void Awake()
    {
        currentHP = maxHP;
    }

    private void Start()
    {
        scoreManager = FindFirstObjectByType<ScoreManager>();
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

        Destroy(gameObject);
    }
}
