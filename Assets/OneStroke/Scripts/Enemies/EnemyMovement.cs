
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.6f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Separation Settings")]
    [SerializeField] private float separationRadius = 1.5f;
    [SerializeField] private float separationStrength = 2.0f;

    private Transform player;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player tag not found!");
        }
    }

    private void Update()
    {
        // ƒJƒEƒ“ƒgƒ_ƒEƒ“’†‚Í“G‚ÌˆÚ“®‚ð’âŽ~
        if (CountdownManager.IsCountingDown)
            return;

        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        // ƒvƒŒƒCƒ„[‚Ö‚Ì’ÇÕ•ûŒü
        Vector3 chaseDirection = Vector3.zero;

        if (distance > stopDistance)
        {
            chaseDirection = direction.normalized;
        }

        // ŽüˆÍ‚Ì“G‚©‚ç—£‚ê‚é•ûŒü‚ðŒvŽZ
        Vector3 separationDirection = Vector3.zero;

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            if (enemy == gameObject)
                continue;

            Vector3 away =
                transform.position - enemy.transform.position;

            away.y = 0f;

            float enemyDistance = away.magnitude;

            if (enemyDistance > 0.001f &&
                enemyDistance < separationRadius)
            {
                // ‹ß‚¢“G‚Ù‚Ç‹­‚­—£‚ê‚æ‚¤‚Æ‚·‚é
                float weight =
                    1f - enemyDistance / separationRadius;

                separationDirection +=
                    away.normalized * weight;
            }
        }

        // ’ÇÕ•ûŒü‚Æ•ª—£•ûŒü‚ð‡¬
        Vector3 moveDirection =
            chaseDirection +
            separationDirection * separationStrength;

        moveDirection = Vector3.ClampMagnitude(moveDirection, 1f);

        // ƒvƒŒƒCƒ„[’ÇÕ‚ð’âŽ~‚µ‚Ä‚¢‚Ä‚à
        // “G“¯Žm‚ª‹ß‚¯‚ê‚Î—£‚ê‚é
        transform.position +=
            moveDirection * moveSpeed * Time.deltaTime;

        // ˆÚ“®•ûŒü‚ðŒü‚­
        if (moveDirection.sqrMagnitude > 0.001f)
        {
            transform.rotation =
                Quaternion.LookRotation(moveDirection);
        }
    }
}
