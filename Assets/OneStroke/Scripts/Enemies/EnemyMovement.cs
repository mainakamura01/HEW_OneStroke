
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float stopDistance = 1.5f;

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
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;

        // 地面の上だけを移動
        direction.y = 0f;

        float distance = direction.magnitude;

        // プレイヤーに近づきすぎないよう停止
        if (distance <= stopDistance)
            return;

        // プレイヤーの方向へ移動
        Vector3 moveDirection = direction.normalized;

        transform.position +=
            moveDirection * moveSpeed * Time.deltaTime;

        // 移動方向を向く
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveDirection);
        }
    }
}
