
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHP = 100;

    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private DamageFlash damageFlash;

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

        // ”í’e‰‰o‚ðÄ¶
        if (damageFlash != null)
        {
            damageFlash.PlayFlash();
        }

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
    }
}
