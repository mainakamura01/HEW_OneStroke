
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
    [SerializeField] private float shapeDisplayTime = 5.0f;

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

        // Å‰‚ÌNode‚É–ß‚Á‚Ä‚«‚½ê‡
        if (activatedNodes.Count >= minimumNodes &&
            activatedNodes[0] == node)
        {
            CompleteShape();
            return;
        }

        // ‚·‚Å‚ÉG‚ê‚½Node‚È‚ç–³‹
        if (activatedNodes.Contains(node))
            return;

        // ’¼‘O‚ÌNode‚ğæ“¾
        Node previousNode = null;

        if (activatedNodes.Count > 0)
        {
            previousNode = activatedNodes[activatedNodes.Count - 1];
        }

        // V‚µ‚¢Node‚ğ“o˜^‚µ‚Ä“_“”
        activatedNodes.Add(node);
        node.Activate();

        // 2‚Â–ÚˆÈ~‚ÌNode‚È‚çü‚ğ¶¬
        if (previousNode != null && lineManager != null)
        {
            lineManager.CreateLine(previousNode, node);
        }

        Debug.Log($"Activated nodes: {activatedNodes.Count}");
    }

    private void CompleteShape()
    {
        isCompletingShape = true;

        Node firstNode = activatedNodes[0];
        Node lastNode = activatedNodes[activatedNodes.Count - 1];

        // ÅŒã‚ÌNode‚ÆÅ‰‚ÌNode‚ğ‚Â‚È‚®
        if (lineManager != null)
        {
            lineManager.CreateLine(lastNode, firstNode);
        }

        Debug.Log($"Shape completed! Nodes: {activatedNodes.Count}");

        // OŠpŒ`‚Ì“à•”‚ğ“h‚è‚Â‚Ô‚·
        if (activatedNodes.Count == 3 && shapeVisualizer != null)
        {
            shapeVisualizer.ShowTriangle(
                activatedNodes[0],
                activatedNodes[1],
                activatedNodes[2]
            );

            Debug.Log("Triangle visualization created!");
        }

        // OŠpŒ`‚Ì“à‘¤‚É‚¢‚é“G‚ğUŒ‚
        if (activatedNodes.Count == 3 && triangleDamage != null)
        {
            triangleDamage.Attack(
                activatedNodes[0],
                activatedNodes[1],
                activatedNodes[2]
            );
        }

        else
        {
            Debug.LogWarning(
                "Triangle visualization skipped. Check node count and ShapeVisualizer reference."
            );
        }

        // ­‚µ‘Ò‚Á‚Ä‚©‚çƒŠƒZƒbƒg
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
        // Node‚ğÂF‚É–ß‚·
        foreach (Node node in activatedNodes)
        {
            if (node != null)
            {
                node.ResetNode();
            }
        }

        activatedNodes.Clear();

        // ü‚ğíœ
        if (lineManager != null)
        {
            lineManager.ClearLines();
        }

        // OŠpŒ`‚Ì“h‚è‚Â‚Ô‚µ‚ğíœ
        if (shapeVisualizer != null)
        {
            shapeVisualizer.ClearShape();
        }

        Debug.Log("Shape reset!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
