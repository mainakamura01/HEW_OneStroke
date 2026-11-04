
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private int currentHP;
    private bool isDead = false;

    public int CurrentHP => currentHP;
    public int MaxHP => maxHP;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int damage)
    {
        if (isDead || damage <= 0)
            return;

        currentHP = Mathf.Max(currentHP - damage, 0);

        Debug.Log($"Player HP: {currentHP} / {maxHP}");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("GAME OVER!");

        if (gameManager != null)
        {
            gameManager.EndGame("GAME OVER");
        }
        else
        {
            Debug.LogWarning(
                "PlayerHealth: GameManager is not assigned!"
            );
        }
    }
}
