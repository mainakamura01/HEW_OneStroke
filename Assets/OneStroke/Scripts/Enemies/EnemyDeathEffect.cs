
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    [Header("Particle Material")]
    [SerializeField] private Material particleMaterial;

    [Header("Particle Colors")]
    [SerializeField]
    private Color greenColor =
        new Color(0.55f, 1f, 0.35f, 1f);

    [SerializeField]
    private Color yellowColor =
        new Color(1f, 0.9f, 0.3f, 1f);

    [Header("Burst Settings")]
    [SerializeField] private int particleCount = 20;
    [SerializeField] private float particleSize = 0.16f;
    [SerializeField] private float particleLifetime = 0.55f;
    [SerializeField] private float burstSpeed = 2f;
    [SerializeField] private float upwardSpeed = 1.5f;
    [SerializeField] private float spawnHeight = 0.6f;

    public void PlayEffect(Vector3 position)
    {
        if (particleMaterial == null)
        {
            Debug.LogWarning(
                "EnemyDeathEffect: Particle Material is missing!"
            );
            return;
        }

        GameObject effectObject =
            new GameObject("EnemyDeathParticles");

        effectObject.transform.position =
            position + Vector3.up * spawnHeight;

        ParticleSystem ps =
            effectObject.AddComponent<ParticleSystem>();

        ps.Stop(
            true,
            ParticleSystemStopBehavior.StopEmittingAndClear
        );

        var main = ps.main;

        main.loop = false;
        main.playOnAwake = false;
        main.duration = 0.1f;
        main.startLifetime = particleLifetime;
        main.startSpeed = 0f;
        main.startSize = particleSize;
        main.startColor = Color.white;
        main.maxParticles = Mathf.Max(1, particleCount);
        main.simulationSpace =
            ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = false;

        var shape = ps.shape;
        shape.enabled = false;

        var renderer =
            ps.GetComponent<ParticleSystemRenderer>();

        renderer.renderMode =
            ParticleSystemRenderMode.Billboard;

        renderer.sharedMaterial = particleMaterial;

        // 粒子の色をシェーダーへ渡す
        renderer.SetActiveVertexStreams(
            new System.Collections.Generic.List<
                ParticleSystemVertexStream>
            {
        ParticleSystemVertexStream.Position,
        ParticleSystemVertexStream.Color,
        ParticleSystemVertexStream.UV
            }
        );

        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = false;

        ps.Play();

        ParticleSystem.EmitParams emitParams =
            new ParticleSystem.EmitParams();

        float lifetime = Mathf.Max(0.05f, particleLifetime);
        int count = Mathf.Max(1, particleCount);

        for (int i = 0; i < count; i++)
        {
            // 水平方向にランダムに飛び散る
            Vector2 randomDirection =
                Random.insideUnitCircle.normalized;

            Vector3 velocity = new Vector3(
                randomDirection.x * burstSpeed,
                upwardSpeed * Random.Range(0.6f, 1.4f),
                randomDirection.y * burstSpeed
            );

            emitParams.position =
                position + Vector3.up * spawnHeight;

            emitParams.velocity = velocity;

            Color selectedColor =
                Random.value < 0.65f
                    ? greenColor
                    : yellowColor;

            emitParams.startColor = selectedColor;

            emitParams.startLifetime = lifetime;

            emitParams.startSize =
                particleSize * Random.Range(0.6f, 1.3f);

            if (i == 0)
            {
                Debug.Log(
                    $"Enemy particle color: {emitParams.startColor}"
                );
            }

            ps.Emit(emitParams, 1);
        }

        // 敵本体が消えても粒子だけ残り、
        // 寿命が終わったら自動で削除する
        Destroy(effectObject, lifetime + 0.3f);
    }
}
