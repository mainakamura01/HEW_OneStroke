
using UnityEngine;

public class NodeSpawnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private StageBoundary stageBoundary;
    [SerializeField] private Transform player;
    [SerializeField] private NodeAppearance[] nodes;

    [Header("Spawn Settings")]
    [SerializeField] private float boundaryMargin = 4f;
    [SerializeField] private float minPlayerDistance = 5f;
    [SerializeField] private float minNodeDistance = 7f;
    [SerializeField] private int maxAttempts = 100;

    public bool TryMoveNode(NodeAppearance targetNode)
    {
        if (targetNode == null ||
            stageBoundary == null ||
            player == null)
        {
            Debug.LogWarning(
                "NodeSpawnManager: References are missing!"
            );
            return false;
        }

        float limitX = stageBoundary.HalfWidth - boundaryMargin;
        float limitZ = stageBoundary.HalfDepth - boundaryMargin;

        if (limitX <= 0f || limitZ <= 0f)
        {
            Debug.LogWarning(
                "NodeSpawnManager: Spawn area is too small!"
            );
            return false;
        }

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 localPosition = new Vector3(
                Random.Range(-limitX, limitX),
                0f,
                Random.Range(-limitZ, limitZ)
            );

            Vector3 candidate =
                stageBoundary.transform.TransformPoint(
                    localPosition
                );

            // ƒm[ƒh‚ÌŒ»Ý‚Ì‚‚³‚ðˆÛŽ
            candidate.y = targetNode.transform.position.y;

            if (!IsFarEnoughFromPlayer(candidate))
                continue;

            if (!IsFarEnoughFromNodes(candidate, targetNode))
                continue;

            targetNode.transform.position = candidate;

            return true;
        }

        Debug.LogWarning(
            $"NodeSpawnManager: Could not find a valid position for {targetNode.name}"
        );

        return false;
    }

    private bool IsFarEnoughFromPlayer(Vector3 candidate)
    {
        Vector2 candidateXZ = new Vector2(
            candidate.x, candidate.z
        );

        Vector2 playerXZ = new Vector2(
            player.position.x, player.position.z
        );

        return Vector2.Distance(
            candidateXZ,
            playerXZ
        ) >= minPlayerDistance;
    }

    private bool IsFarEnoughFromNodes(
        Vector3 candidate,
        NodeAppearance targetNode)
    {
        foreach (NodeAppearance otherNode in nodes)
        {
            if (otherNode == null || otherNode == targetNode)
                continue;

            Vector2 candidateXZ = new Vector2(
                candidate.x, candidate.z
            );

            Vector2 otherXZ = new Vector2(
                otherNode.transform.position.x,
                otherNode.transform.position.z
            );

            if (Vector2.Distance(candidateXZ, otherXZ)
                < minNodeDistance)
            {
                return false;
            }
        }

        return true;
    }
}
