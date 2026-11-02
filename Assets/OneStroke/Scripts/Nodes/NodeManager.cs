
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private LineManager lineManager;

    private readonly List<Node> activatedNodes = new();

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

    public void RegisterNode(Node node)
    {
        if (node == null || activatedNodes.Contains(node))
            return;

        // ’¼‘O‚É“_“”‚µ‚½Node‚ğæ“¾
        Node previousNode = null;

        if (activatedNodes.Count > 0)
        {
            previousNode = activatedNodes[activatedNodes.Count - 1];
        }

        // V‚µ‚¢Node‚ğ“o˜^
        activatedNodes.Add(node);

        Debug.Log($"Activated nodes: {activatedNodes.Count}");

        // 2ŒÂ–ÚˆÈ~‚È‚çü‚ğ¶¬
        if (previousNode != null && lineManager != null)
        {
            lineManager.CreateLine(previousNode, node);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
