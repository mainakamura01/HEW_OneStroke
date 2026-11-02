
using UnityEngine;

public class ShapeVisualizer : MonoBehaviour
{
    [Header("Shape Settings")]
    [SerializeField] private Material shapeMaterial;
    [SerializeField] private float shapeHeight = 0.12f;

    private GameObject currentShape;

    public void ShowTriangle(Node a, Node b, Node c)
    {
        ClearShape();

        currentShape = new GameObject("CompletedTriangle");
        currentShape.transform.SetParent(transform);

        MeshFilter meshFilter =
            currentShape.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer =
            currentShape.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();

        Vector3[] vertices =
        {
            new Vector3(a.transform.position.x, shapeHeight,
                        a.transform.position.z),

            new Vector3(b.transform.position.x, shapeHeight,
                        b.transform.position.z),

            new Vector3(c.transform.position.x, shapeHeight,
                        c.transform.position.z)
        };

        mesh.vertices = vertices;

        // •\— ‚Ç‚¿‚ç‚©‚ç‚à•`‰æ‚µ‚â‚·‚¢‚æ‚¤—¼–Ê‚ð’è‹`
        mesh.triangles = new int[]
        {
            0, 1, 2,
            2, 1, 0
        };

        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;

        if (shapeMaterial != null)
            meshRenderer.material = shapeMaterial;
    }

    public void ClearShape()
    {
        if (currentShape != null)
        {
            Destroy(currentShape);
            currentShape = null;
        }
    }
}
