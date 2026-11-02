
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    private Renderer nodeRenderer;
    private bool isActivated = false;

    public bool IsActivated => isActivated;

    private void Awake()
    {
        nodeRenderer = GetComponent<Renderer>();

        if (inactiveMaterial != null)
            nodeRenderer.material = inactiveMaterial;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CharacterController>() == null)
            return;

        // ì_ìîçœÇ›Ç≈Ç‡NodeManagerÇ÷í ímÇ∑ÇÈ
        NodeManager.Instance?.OnNodeTouched(this);
    }

    public void Activate()
    {
        if (isActivated)
            return;

        isActivated = true;

        if (activeMaterial != null)
            nodeRenderer.material = activeMaterial;
    }

    public void ResetNode()
    {
        isActivated = false;

        if (inactiveMaterial != null)
            nodeRenderer.material = inactiveMaterial;
    }
}
