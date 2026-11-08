
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
    [SerializeField] private PolygonParticleEffect polygonParticleEffect;

    [Header("Shape Settings")]
    [SerializeField] private int minimumNodes = 3;

    [Header("Animation Settings")]
    [SerializeField] private float shapeDisplayTime = 0.8f;

    [Header("Attack Timing")]
    [SerializeField] private float attackDelay = 0.35f;

    private readonly List<Node> activatedNodes = new();

    private bool isCompletingShape = false;

    public IReadOnlyList<Node> ActivatedNodes => activatedNodes;

    [SerializeField] private InvalidShapeUI invalidShapeUI;

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

        if (activatedNodes.Count >= minimumNodes && activatedNodes[0] == node)
        {
            if (!IsValidPolygon())
            {
                Debug.LogWarning(
                    "Invalid polygon: crossing lines or zero area."
                );

                if (invalidShapeUI != null)
                {
                    invalidShapeUI.ShowWarning(
                        "線が交差しているか、図形を作れません！\n右クリックで1つ戻ろう"
                    );
                }

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

        // 図形完成時の位置で攻撃対象を確定する
        List<Enemy> lockedTargets = new();

        if (triangleDamage != null)
        {
            lockedTargets =
                triangleDamage.FindTargets(activatedNodes);
        }

        if (lineManager != null)
        {
            // 最後の線をつないで図形を閉じる
            lineManager.CreateLine(lastNode, firstNode);

            // 元の線を暗くする
            lineManager.DimCompletedLines();

            // 二重発光で一周する
            lineManager.PlayTravelingLight();
        }

        Debug.Log(
            $"Polygon completed! Nodes: {activatedNodes.Count}"
        );

        // 内部表示と攻撃は、光が一周した後に実行
        StartCoroutine(
            CompleteShapeAnimation(lockedTargets)
        );
    }

    private IEnumerator CompleteShapeAnimation(
        List<Enemy> lockedTargets)
    {
        // 二重発光が一周するのを待つ
        float delay = Mathf.Clamp(
            attackDelay,
            0f,
            Mathf.Max(0f, shapeDisplayTime)
        );

        yield return new WaitForSeconds(delay);

        // 図形が光って攻撃する瞬間に効果音！
        GameSoundManager.Instance?.PlayPolygonComplete();

        // ここで初めて図形内部を表示
        if (shapeVisualizer != null)
        {
            shapeVisualizer.ShowPolygon(activatedNodes);
        }

        if (polygonParticleEffect != null)
        {
            polygonParticleEffect.PlayBurst(activatedNodes);
        }

        // 完成時に確定していた敵にダメージ
        if (triangleDamage != null)
        {
            triangleDamage.DamageTargets(lockedTargets);
        }

        Debug.Log("Polygon attack activated!");

        // 図形完成から合計shapeDisplayTime秒で消す
        float remainingTime =
            Mathf.Max(0f, shapeDisplayTime - delay);

        if (remainingTime > 0f)
        {
            yield return new WaitForSeconds(remainingTime);
        }

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
