
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private int attackDamage = 5;
    [SerializeField] private float attackRange = 1.7f;
    [SerializeField] private float attackInterval = 1.5f;

    private Transform player;
    private PlayerHealth playerHealth;

    private float attackTimer = 0f;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogWarning("Player not found!");
            return;
        }

        player = playerObject.transform;
        playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            Debug.LogWarning("PlayerHealth component not found!");
        }
    }

    private void Update()
    {
        if (player == null || playerHealth == null)
            return;

        if (playerHealth.IsDead)
            return;

        // 攻撃のクールタイムを進める
        if (attackTimer > 0f)
        {
            attackTimer -= Time.deltaTime;
        }

        // プレイヤーとの水平距離を計算
        Vector3 difference = player.position - transform.position;
        difference.y = 0f;

        float distance = difference.magnitude;

        // 攻撃範囲外なら攻撃しない
        if (distance > attackRange)
            return;

        // クールタイムが終わっていたら攻撃
        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackInterval;
        }
    }

    private void Attack()
    {
        Debug.Log($"{gameObject.name} attacks!");

        playerHealth.TakeDamage(attackDamage);
    }
}
