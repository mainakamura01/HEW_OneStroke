
using System.Collections.Generic;
using UnityEngine;

public class PolygonParticleEffect : MonoBehaviour
{
    [Header("Particle Appearance")]
    [SerializeField] private Material particleMaterial;
    [SerializeField]
    private Color particleColor =
        new Color(0.4f, 0.95f, 1f, 1f);

    [SerializeField] private float particleSize = 0.14f;
    [SerializeField] private float particleLifetime = 0.45f;

    [Header("Burst Settings")]
    [SerializeField] private float particlesPerUnit = 5f;
    [SerializeField] private int maxParticles = 150;
    [SerializeField] private float spawnHeight = 0.2f;
    [SerializeField] private float upwardSpeed = 2f;
    [SerializeField] private float sidewaysSpeed = 0.3f;

    public void PlayBurst(IReadOnlyList<Node> nodes)
    {
        if (nodes == null || nodes.Count < 3)
            return;

        if (particleMaterial == null)
        {
            Debug.LogWarning(
                "PolygonParticleEffect: Particle Material is missing."
            );
            return;
        }

        // 図形の輪郭を取得
        List<Vector3> points = new();

        foreach (Node node in nodes)
        {
            if (node == null)
                return;

            points.Add(node.transform.position);
        }

        // 各辺の長さを計算
        float perimeter = 0f;

        for (int i = 0; i < points.Count; i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[(i + 1) % points.Count];

            perimeter += Vector3.Distance(a, b);
        }

        int count = Mathf.Clamp(
            Mathf.CeilToInt(perimeter * particlesPerUnit),
            1,
            maxParticles
        );

        // 一回の攻撃ごとに独立した粒子を生成
        GameObject effectObject =
            new GameObject("PolygonParticleBurst");

        effectObject.transform.position = Vector3.zero;

        ParticleSystem ps =
            effectObject.AddComponent<ParticleSystem>();

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.1f;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0f;
        main.startSize = particleSize;
        main.startColor = particleColor;
        main.maxParticles = maxParticles;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.enabled = false;

        var renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        renderer.renderMode =
            ParticleSystemRenderMode.Billboard;

        renderer.sharedMaterial = particleMaterial;

        // 粒子を図形の各辺に沿って均等に配置
        ParticleSystem.EmitParams emitParams =
            new ParticleSystem.EmitParams();

        ps.Play();

        float lifetime = Mathf.Max(0.05f, particleLifetime);

        for (int i = 0; i < count; i++)
        {
            // 輪郭上の位置を等間隔で選ぶ
            float distance = perimeter * (i + 0.5f) / count;

            Vector3 position =
                GetPointOnPerimeter(points, distance);

            position.y = spawnHeight;

            // 少しランダムに上へ飛ぶ
            Vector3 velocity = new Vector3(
                Random.Range(-sidewaysSpeed, sidewaysSpeed),
                upwardSpeed * Random.Range(0.7f, 1.3f),
                Random.Range(-sidewaysSpeed, sidewaysSpeed)
            );

            emitParams.position = position;
            emitParams.velocity = velocity;
            emitParams.startLifetime = lifetime;
            emitParams.startSize =
                particleSize * Random.Range(0.6f, 1.4f);

            ps.Emit(emitParams, 1);
        }

        // 粒子が消えたらオブジェクトも削除
        Destroy(effectObject, lifetime + 0.3f);
    }

    private Vector3 GetPointOnPerimeter(
        List<Vector3> points,
        float distance)
    {
        for (int i = 0; i < points.Count; i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[(i + 1) % points.Count];

            float length = Vector3.Distance(a, b);

            if (distance <= length)
            {
                float t = distance /
                    Mathf.Max(0.001f, length);

                return Vector3.Lerp(a, b, t);
            }

            distance -= length;
        }

        return points[0];
    }
}
