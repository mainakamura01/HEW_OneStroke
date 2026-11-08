
using UnityEngine;

[RequireComponent(typeof(Node))]
public class NodeProximityEffect : MonoBehaviour
{
    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 2.0f;
    [SerializeField] private float enlargedScale = 1.2f;
    [SerializeField] private float animationSpeed = 8f;

    private Transform player;
    private Node node;
    private NodeAppearance nodeAppearance;

    private Renderer nodeRenderer;
    private Vector3 originalVisualScale;

    private void Start()
    {
        node = GetComponent<Node>();
        nodeAppearance = GetComponent<NodeAppearance>();
        nodeRenderer = GetComponent<Renderer>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        if (nodeRenderer != null)
        {
            originalVisualScale = transform.localScale;
        }
    }

    private void Update()
    {
        if (player == null || nodeRenderer == null)
            return;

        // 出現していないノードには反応しない
        if (nodeAppearance != null && !nodeAppearance.IsVisible)
            return;

        Vector3 nodePosition = transform.position;
        Vector3 playerPosition = player.position;

        // 水平方向の距離だけを見る
        nodePosition.y = 0f;
        playerPosition.y = 0f;

        float distance = Vector3.Distance(
            nodePosition,
            playerPosition
        );

        bool isNear =
            distance <= detectionRadius &&
            !node.IsActivated;

        float targetScale = isNear ? enlargedScale : 1f;

        // 現在のスケールから滑らかに変化
        transform.localScale = Vector3.Lerp(
            transform.localScale,
            originalVisualScale * targetScale,
            Time.deltaTime * animationSpeed
        );
    }
}
