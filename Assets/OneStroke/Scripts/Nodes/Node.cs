
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Node : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material inactiveMaterial;
    [SerializeField] private Material activeMaterial;

    [Header("Visual Reference")]
    [SerializeField] private Renderer nodeVisualRenderer;

    private bool isActivated = false;

    public bool IsActivated => isActivated;

    [Header("Activation Effect")]
    [SerializeField] private NodeActivationEffect activationEffect;

    private void Awake()
    {
        if (nodeVisualRenderer == null)
        {
            Transform visual = transform.Find("NodeVisual");

            if (visual != null)
            {
                nodeVisualRenderer =
                    visual.GetComponent<Renderer>();
            }
        }

        if (nodeVisualRenderer != null &&
            inactiveMaterial != null)
        {
            nodeVisualRenderer.material = inactiveMaterial;
        }

        if (activationEffect == null)
        {
            activationEffect =
                GetComponentInChildren<NodeActivationEffect>(true);
        }
    }

    // Nodeの起動をNodeManagerに依頼
    public void Interact()
    {
        NodeManager.Instance?.OnNodeTouched(this);
    }

    public void Activate()
    {
        if (isActivated)
            return;

        isActivated = true;

        GameSoundManager.Instance?.PlayNodeActivate();

        if (nodeVisualRenderer != null &&
            activeMaterial != null)
        {
            nodeVisualRenderer.material = activeMaterial;
        }

        // 赤くなった瞬間にエフェクト再生！
        if (activationEffect != null)
        {
            activationEffect.PlayEffect();
        }
    }

    public void ResetNode()
    {
        isActivated = false;

        if (nodeVisualRenderer != null &&
            inactiveMaterial != null)
        {
            nodeVisualRenderer.material = inactiveMaterial;
        }
    }
}
