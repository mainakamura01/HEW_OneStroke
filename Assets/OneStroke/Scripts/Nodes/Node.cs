
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
        {
            nodeRenderer.material = inactiveMaterial;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // PlayerˆÈŠO‚Æ‚ÌÚG‚Í–³‹
        if (other.GetComponent<CharacterController>() == null)
            return;

        Activate();
    }

    public void Activate()
    {
        // ‚·‚Å‚É“_“”‚µ‚Ä‚¢‚éê‡‚Í‰½‚à‚µ‚È‚¢
        if (isActivated)
            return;

        isActivated = true;

        if (activeMaterial != null)
        {
            nodeRenderer.material = activeMaterial;
        }

        NodeManager.Instance?.RegisterNode(this);

        Debug.Log($"Node activated: {gameObject.name}");
    }
}
