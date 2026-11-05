
using System.Collections.Generic;
using UnityEngine;

public class TriangleDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;

    public void AttackPolygon(IReadOnlyList<Node> nodes)
    {
        if (nodes == null || nodes.Count < 3)
            return;

        List<Vector2> polygon = new List<Vector2>();

        foreach (Node node in nodes)
        {
            Vector3 p = node.transform.position;
            polygon.Add(new Vector2(p.x, p.z));
        }

        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            Vector3 position = enemy.transform.position;
            Vector2 point = new Vector2(
                position.x,
                position.z
            );

            if (IsInsidePolygon(point, polygon))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private bool IsInsidePolygon(
        Vector2 point,
        List<Vector2> polygon)
    {
        bool inside = false;

        int count = polygon.Count;

        for (int i = 0, j = count - 1;
             i < count;
             j = i++)
        {
            Vector2 a = polygon[i];
            Vector2 b = polygon[j];

            // “_‚ª•Óã‚É‚ ‚éê‡‚à“à‘¤‚Æ‚µ‚Äˆµ‚¤
            if (IsPointOnSegment(point, a, b))
                return true;

            bool intersects =
                ((a.y > point.y) != (b.y > point.y)) &&
                (point.x <
                 (b.x - a.x) *
                 (point.y - a.y) /
                 (b.y - a.y) + a.x);

            if (intersects)
                inside = !inside;
        }

        return inside;
    }

    private bool IsPointOnSegment(
        Vector2 p,
        Vector2 a,
        Vector2 b)
    {
        float cross =
            (b.x - a.x) * (p.y - a.y) -
            (b.y - a.y) * (p.x - a.x);

        if (Mathf.Abs(cross) > 0.01f)
            return false;

        float dot = Vector2.Dot(p - a, b - a);

        if (dot < 0f)
            return false;

        float lengthSquared = (b - a).sqrMagnitude;

        return dot <= lengthSquared;
    }
}
