
using UnityEngine;

public class TriangleDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private int damage = 1;

    public void Attack(Node a, Node b, Node c)
    {
        // 3D座標を床の2D座標(XZ)に変換
        Vector2 pointA = ToXZ(a.transform.position);
        Vector2 pointB = ToXZ(b.transform.position);
        Vector2 pointC = ToXZ(c.transform.position);

        // シーン内の敵を探す
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemyObject in enemies)
        {
            Enemy enemy = enemyObject.GetComponent<Enemy>();

            if (enemy == null)
                continue;

            Vector2 enemyPosition = ToXZ(enemy.transform.position);

            if (IsInsideTriangle(
                enemyPosition,
                pointA,
                pointB,
                pointC
            ))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private Vector2 ToXZ(Vector3 position)
    {
        return new Vector2(position.x, position.z);
    }

    // 2Dベクトルの外積
    private float Cross(Vector2 a, Vector2 b, Vector2 p)
    {
        return (b.x - a.x) * (p.y - a.y)
             - (b.y - a.y) * (p.x - a.x);
    }

    private bool IsInsideTriangle(
        Vector2 p,
        Vector2 a,
        Vector2 b,
        Vector2 c
    )
    {
        float d1 = Cross(a, b, p);
        float d2 = Cross(b, c, p);
        float d3 = Cross(c, a, p);

        // 3点がほぼ一直線なら三角形ではない
        if (Mathf.Abs(Cross(a, b, c)) < 0.0001f)
        {
            return false;
        }

        bool hasNegative =
            d1 < -0.0001f ||
            d2 < -0.0001f ||
            d3 < -0.0001f;

        bool hasPositive =
            d1 > 0.0001f ||
            d2 > 0.0001f ||
            d3 > 0.0001f;

        // 全て同じ側なら内側
        return !(hasNegative && hasPositive);
    }
}
