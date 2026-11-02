
using UnityEngine;

public class NodeInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2.0f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }
    }


    private void TryInteract()
    {
        GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

        Node nearestNode = null;
        float nearestDistance = interactionRange;

        foreach (GameObject nodeObject in nodes)
        {
            Node node = nodeObject.GetComponent<Node>();

            if (node == null)
                continue;

            NodeAppearance appearance = nodeObject.GetComponent<NodeAppearance>();

            if (appearance != null && !appearance.IsVisible)
            {
                continue;
            }

            float distance = Vector3.Distance(
                transform.position,
                node.transform.position
            );

            if (distance <= nearestDistance)
            {
                nearestDistance = distance;
                nearestNode = node;
            }
        }

        if (nearestNode != null)
        {
            nearestNode.Interact();
        }
    }

}
