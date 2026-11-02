
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.12f;
    [SerializeField] private float lineHeight = 0.15f;

    private readonly List<GameObject> lines = new();

    public void CreateLine(Node startNode, Node endNode)
    {
        if (startNode == null || endNode == null)
            return;

        GameObject lineObject = new GameObject(
            $"Line_{startNode.name}_{endNode.name}"
        );

        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;

        if (lineMaterial != null)
            line.material = lineMaterial;

        Vector3 startPosition = startNode.transform.position;
        Vector3 endPosition = endNode.transform.position;

        startPosition.y = lineHeight;
        endPosition.y = lineHeight;

        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        lines.Add(lineObject);

        Debug.Log($"Line created: {startNode.name} -> {endNode.name}");
    }

    public void ClearLines()
    {
        foreach (GameObject line in lines)
        {
            if (line != null)
                Destroy(line);
        }

        lines.Clear();
    }
}
