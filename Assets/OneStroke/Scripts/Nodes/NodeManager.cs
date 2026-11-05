
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LineManager lineManager;
    [SerializeField] private ShapeVisualizer shapeVisualizer;
    [SerializeField] private TriangleDamage triangleDamage;

    [Header("Shape Settings")]
    [SerializeField] private int minimumNodes = 3;

    [Header("Animation Settings")]
    [SerializeField] private float shapeDisplayTime = 0.8f;

    private readonly List<Node> activatedNodes = new();

    private bool isCompletingShape = false;

    public IReadOnlyList<Node> ActivatedNodes => activatedNodes;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnNodeTouched(Node node)
    {
        if (node == null || isCompletingShape)
            return;

        // 最初のNodeをもう一度選択すると完成
        if (activatedNodes.Count >= minimumNodes &&
            activatedNodes[0] == node)
        {
            if (!IsValidPolygon())
            {
                Debug.LogWarning(
                    "Invalid polygon: crossing lines or zero area."
                );
                return;
            }

            CompleteShape();
            return;
        }

        // すでに選択済みのNodeは無視
        if (activatedNodes.Contains(node))
            return;

        Node previousNode = activatedNodes.Count > 0
            ? activatedNodes[activatedNodes.Count - 1]
            : null;

        activatedNodes.Add(node);
        node.Activate();

        if (previousNode != null && lineManager != null)
        {
            lineManager.CreateLine(previousNode, node);
        }

        Debug.Log($"Activated nodes: {activatedNodes.Count}");
    }

    public void UndoLastNode()
    {
        // 図形完成中は操作を受け付けない
        if (isCompletingShape)
            return;

        if (activatedNodes.Count == 0)
            return;

        int lastIndex = activatedNodes.Count - 1;

        Node lastNode = activatedNodes[lastIndex];

        // 最後のノードをリストから削除
        activatedNodes.RemoveAt(lastIndex);

        // ノードを青色に戻す
        if (lastNode != null)
        {
            lastNode.ResetNode();

            NodeAppearance appearance =
                lastNode.GetComponent<NodeAppearance>();

            if (appearance != null)
            {
                appearance.RestartVisibility();
            }
        }

        // 2つ以上選択されていた場合、最後の線を削除
        if (activatedNodes.Count > 0 && lineManager != null)
            lineManager.RemoveLastLine();

        Debug.Log(
            $"Undo! Remaining nodes: {activatedNodes.Count}"
        );
    }

    public void CancelShape()
    {
        // 図形完成中は操作を受け付けない
        if (isCompletingShape)
            return;

        if (activatedNodes.Count == 0)
            return;

        ResetShape();

        Debug.Log("Shape selection cancelled!");
    }


    private void CompleteShape()
    {
        isCompletingShape = true;

        Node firstNode = activatedNodes[0];
        Node lastNode = activatedNodes[activatedNodes.Count - 1];

        if (lineManager != null)
        {
            lineManager.CreateLine(lastNode, firstNode);
        }

        Debug.Log(
            $"Polygon completed! Nodes: {activatedNodes.Count}"
        );

        if (shapeVisualizer != null)
        {
            shapeVisualizer.ShowPolygon(activatedNodes);
        }

        if (triangleDamage != null)
        {
            triangleDamage.AttackPolygon(activatedNodes);
        }

        StartCoroutine(CompleteShapeAnimation());
    }

    private IEnumerator CompleteShapeAnimation()
    {
        yield return new WaitForSeconds(shapeDisplayTime);

        ResetShape();
        isCompletingShape = false;
    }

    private void ResetShape()
    {
        foreach (Node node in activatedNodes)
        {
            if (node != null)
                node.ResetNode();
        }

        activatedNodes.Clear();

        if (lineManager != null)
            lineManager.ClearLines();

        if (shapeVisualizer != null)
            shapeVisualizer.ClearShape();

        Debug.Log("Shape reset!");
    }

    private bool IsValidPolygon()
    {
        if (activatedNodes.Count < 3)
            return false;

        List<Vector2> points = new List<Vector2>();

        foreach (Node node in activatedNodes)
        {
            Vector3 p = node.transform.position;
            points.Add(new Vector2(p.x, p.z));
        }

        // 面積がほぼゼロなら無効
        float area = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % points.Count];

            area += a.x * b.y - b.x * a.y;
        }

        if (Mathf.Abs(area) < 0.01f)
            return false;

        // 隣接しない辺同士の交差を確認
        int count = points.Count;

        for (int i = 0; i < count; i++)
        {
            Vector2 a1 = points[i];
            Vector2 a2 = points[(i + 1) % count];

            for (int j = i + 1; j < count; j++)
            {
                if (j == i ||
                    j == (i + 1) % count ||
                    (j + 1) % count == i)
                    continue;

                Vector2 b1 = points[j];
                Vector2 b2 = points[(j + 1) % count];

                if (SegmentsIntersect(a1, a2, b1, b2))
                    return false;
            }
        }

        return true;
    }

    private bool SegmentsIntersect(
        Vector2 a, Vector2 b,
        Vector2 c, Vector2 d)
    {
        float d1 = Cross(a, b, c);
        float d2 = Cross(a, b, d);
        float d3 = Cross(c, d, a);
        float d4 = Cross(c, d, b);

        return d1 * d2 < 0f && d3 * d4 < 0f;
    }

    private float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y)
             - (b.y - a.y) * (c.x - a.x);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
