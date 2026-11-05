
using System.Collections.Generic;
using UnityEngine;

public class ShapeVisualizer : MonoBehaviour
{
    [Header("Shape Settings")]
    [SerializeField] private Material shapeMaterial;
    [SerializeField] private float shapeHeight = 0.12f;

    private GameObject currentShape;

    public void ShowPolygon(IReadOnlyList<Node> nodes)
    {
        ClearShape();

        if (nodes == null || nodes.Count < 3)
            return;

        List<Vector2> points = new List<Vector2>();

        foreach (Node node in nodes)
        {
            Vector3 p = node.transform.position;
            points.Add(new Vector2(p.x, p.z));
        }

        List<int> triangles = Triangulate(points);

        if (triangles.Count == 0)
        {
            Debug.LogWarning(
                "ShapeVisualizer: Could not create polygon mesh."
            );
            return;
        }

        currentShape = new GameObject("CompletedPolygon");
        currentShape.transform.SetParent(transform);

        MeshFilter meshFilter =
            currentShape.AddComponent<MeshFilter>();

        MeshRenderer meshRenderer =
            currentShape.AddComponent<MeshRenderer>();

        Vector3[] vertices = new Vector3[points.Count];

        for (int i = 0; i < points.Count; i++)
        {
            vertices[i] = new Vector3(
                points[i].x,
                shapeHeight,
                points[i].y
            );
        }

        // •\— —¼–Ê‚ðì‚é
        List<int> doubleSided = new List<int>(triangles);

        for (int i = 0; i < triangles.Count; i += 3)
        {
            doubleSided.Add(triangles[i + 2]);
            doubleSided.Add(triangles[i + 1]);
            doubleSided.Add(triangles[i]);
        }

        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = doubleSided.ToArray();
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        if (shapeMaterial != null)
            meshRenderer.material = shapeMaterial;
    }

    private List<int> Triangulate(List<Vector2> points)
    {
        List<int> result = new List<int>();
        List<int> remaining = new List<int>();

        for (int i = 0; i < points.Count; i++)
            remaining.Add(i);

        bool counterClockwise = SignedArea(points) > 0f;

        int safety = 0;

        while (remaining.Count > 3 && safety < 1000)
        {
            safety++;
            bool earFound = false;

            for (int i = 0; i < remaining.Count; i++)
            {
                int prev = remaining[
                    (i - 1 + remaining.Count) % remaining.Count
                ];

                int curr = remaining[i];

                int next = remaining[
                    (i + 1) % remaining.Count
                ];

                float cross = Cross(
                    points[prev],
                    points[curr],
                    points[next]
                );

                if (counterClockwise
                    ? cross <= 0.0001f
                    : cross >= -0.0001f)
                {
                    continue;
                }

                bool containsPoint = false;

                foreach (int index in remaining)
                {
                    if (index == prev ||
                        index == curr ||
                        index == next)
                        continue;

                    if (PointInTriangle(
                        points[index],
                        points[prev],
                        points[curr],
                        points[next]
                    ))
                    {
                        containsPoint = true;
                        break;
                    }
                }

                if (containsPoint)
                    continue;

                result.Add(prev);
                result.Add(curr);
                result.Add(next);

                remaining.RemoveAt(i);
                earFound = true;
                break;
            }

            if (!earFound)
                return new List<int>();
        }

        if (remaining.Count == 3)
        {
            result.Add(remaining[0]);
            result.Add(remaining[1]);
            result.Add(remaining[2]);
        }

        return result;
    }

    private float SignedArea(List<Vector2> points)
    {
        float area = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector2 a = points[i];
            Vector2 b = points[(i + 1) % points.Count];

            area += a.x * b.y - b.x * a.y;
        }

        return area * 0.5f;
    }

    private float Cross(Vector2 a, Vector2 b, Vector2 c)
    {
        return (b.x - a.x) * (c.y - a.y)
             - (b.y - a.y) * (c.x - a.x);
    }

    private bool PointInTriangle(
        Vector2 p,
        Vector2 a,
        Vector2 b,
        Vector2 c)
    {
        float d1 = Cross(a, b, p);
        float d2 = Cross(b, c, p);
        float d3 = Cross(c, a, p);

        bool hasNegative =
            d1 < -0.0001f ||
            d2 < -0.0001f ||
            d3 < -0.0001f;

        bool hasPositive =
            d1 > 0.0001f ||
            d2 > 0.0001f ||
            d3 > 0.0001f;

        return !(hasNegative && hasPositive);
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
