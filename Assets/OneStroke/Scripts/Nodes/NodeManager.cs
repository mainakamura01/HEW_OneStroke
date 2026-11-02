
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public static NodeManager Instance { get; private set; }

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

        activatedNodes.Add(node);

        Debug.Log($"Activated nodes: {activatedNodes.Count}");
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
