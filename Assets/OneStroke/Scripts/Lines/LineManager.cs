
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.12f;
    [SerializeField] private float lineHeight = 0.15f;

    public void CreateLine(Node startNode, Node endNode)
    {
        if (startNode == null || endNode == null)
            return;

        // ü‚ğŠÇ—‚·‚éGameObject‚ğ¶¬
        GameObject lineObject = new GameObject(
            $"Line_{startNode.name}_{endNode.name}"
        );

        // LineManager‚Ìq‚É‚·‚é
        lineObject.transform.SetParent(transform);

        // LineRenderer‚ğ’Ç‰Á
        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        // ü‚Ìİ’è
        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        line.numCapVertices = 4;
        line.numCornerVertices = 4;

        if (lineMaterial != null)
        {
            line.material = lineMaterial;
        }

        // °‚É–„‚Ü‚ç‚È‚¢‚æ‚¤­‚µã‚É”z’u
        Vector3 startPosition = startNode.transform.position;
        Vector3 endPosition = endNode.transform.position;

        startPosition.y = lineHeight;
        endPosition.y = lineHeight;

        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        Debug.Log(
            $"Line created: {startNode.name} -> {endNode.name}"
        );
    }
}
