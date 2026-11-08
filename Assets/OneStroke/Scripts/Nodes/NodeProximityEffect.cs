
using UnityEngine;

[RequireComponent(typeof(Node))]
public class NodeProximityEffect : MonoBehaviour
{
    [Header("Visual References")]
    [SerializeField] private Transform nodeVisual;
    [SerializeField] private NodeGlowRing glowRing;

    [Header("Proximity Settings")]
    [SerializeField] private float detectionRadius = 2.0f;
    [SerializeField] private float enlargedScale = 1.2f;
    [SerializeField] private float animationSpeed = 8f;

    private Transform player;
    private Node node;
    private NodeAppearance nodeAppearance;

    private Vector3 originalVisualScale;

    private void Start()
    {
        node = GetComponent<Node>();
        nodeAppearance = GetComponent<NodeAppearance>();

        if (nodeVisual == null)
            nodeVisual = transform.Find("NodeVisual");

        if (glowRing == null)
            glowRing = GetComponentInChildren<NodeGlowRing>();

        if (nodeVisual != null)
            originalVisualScale = nodeVisual.localScale;

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
    }

    private void Update()
    {
        if (player == null || nodeVisual == null)
            return;

        // カウントダウン中は反応しない
        if (CountdownManager.IsCountingDown)
        {
            if (glowRing != null)
                glowRing.SetVisible(false);

            return;
        }

        // 非表示中はリングを消す
        if (nodeAppearance != null &&
            !nodeAppearance.IsVisible)
        {
            nodeVisual.localScale = originalVisualScale;

            if (glowRing != null)
                glowRing.SetVisible(false);

            return;
        }

        Vector3 nodePosition = transform.position;
        Vector3 playerPosition = player.position;

        nodePosition.y = 0f;
        playerPosition.y = 0f;

        float distance =
            Vector3.Distance(nodePosition, playerPosition);

        bool isNear =
            distance <= detectionRadius &&
            !node.IsActivated;

        Vector3 targetScale =
            originalVisualScale *
            (isNear ? enlargedScale : 1f);

        nodeVisual.localScale = Vector3.Lerp(
            nodeVisual.localScale,
            targetScale,
            Time.deltaTime * animationSpeed
        );

        // 近づいたらリングを表示
        if (glowRing != null)
            glowRing.SetVisible(isNear);
    }
}
