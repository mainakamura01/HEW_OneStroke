
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;
    [SerializeField] private StageBoundary stageBoundary;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 8f;
    [SerializeField] private float boundaryMargin = 1.5f;
    [SerializeField] private int spawnPositionAttempts = 30;

    [Header("Normal Wave")]
    [SerializeField] private int normalEnemyCount = 4;
    [SerializeField] private float normalCheckInterval = 5f;

    [Header("Enhanced Wave")]
    [SerializeField] private int enhancedEnemyCount = 12;
    [SerializeField] private float enhancedSpawnInterval = 15f;
    [SerializeField] private int enhancedAdditionalCount = 4;

    [Header("Wave Timing")]
    [SerializeField] private float normalDuration = 30f;
    [SerializeField] private float enhancedDuration = 60f;

    private bool isEnhancedWave = false;

    public bool IsEnhancedWave => isEnhancedWave;

    private void Start()
    {
        StartCoroutine(WaveRoutine());
    }

    private IEnumerator WaveRoutine()
    {
        if (enemyPrefab == null ||
            player == null ||
            stageBoundary == null)
        {
            Debug.LogError(
                "EnemySpawner: References are missing!"
            );
            yield break;
        }

        while (true)
        {
            // 通常ウェーブ
            isEnhancedWave = false;
            Debug.Log("通常ウェーブ開始！");

            SpawnUntilCount(normalEnemyCount);

            float elapsed = 0f;

            while (elapsed < normalDuration)
            {
                yield return new WaitForSeconds(
                    normalCheckInterval
                );

                elapsed += normalCheckInterval;

                if (elapsed < normalDuration)
                {
                    SpawnUntilCount(normalEnemyCount);
                }
            }

            // 強化ウェーブ
            isEnhancedWave = true;
            Debug.Log("強化ウェーブ開始！");

            SpawnUntilCount(enhancedEnemyCount);

            elapsed = 0f;

            while (elapsed < enhancedDuration)
            {
                yield return new WaitForSeconds(
                    enhancedSpawnInterval
                );

                elapsed += enhancedSpawnInterval;

                if (elapsed < enhancedDuration)
                {
                    SpawnEnemies(enhancedAdditionalCount);
                }
            }

            isEnhancedWave = false;
            Debug.Log(
                "強化ウェーブ終了！通常に戻ります"
            );
        }
    }

    private void SpawnUntilCount(int targetCount)
    {
        int currentCount =
            GameObject.FindGameObjectsWithTag("Enemy").Length;

        int missingCount =
            Mathf.Max(0, targetCount - currentCount);

        SpawnEnemies(missingCount);
    }

    private void SpawnEnemies(int count)
    {
        if (count <= 0)
            return;

        float startAngle = Random.Range(0f, 360f);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition =
                FindValidSpawnPosition(
                    startAngle + (360f / count) * i
                );

            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        Debug.Log(
            $"{count}体の敵を出現させました！" +
            $" 現在のウェーブ: " +
            (isEnhancedWave ? "強化" : "通常")
        );
    }

    private Vector3 FindValidSpawnPosition(float baseAngle)
    {
        // 最初は等間隔の方向を試す
        // ダメなら別の方向をランダムに試す
        for (int attempt = 0;
             attempt < spawnPositionAttempts;
             attempt++)
        {
            float angle = attempt == 0
                ? baseAngle
                : Random.Range(0f, 360f);

            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                0f,
                Mathf.Sin(radians)
            );

            Vector3 candidate =
                player.position + direction * spawnDistance;

            if (IsInsideStage(candidate))
            {
                return candidate;
            }
        }

        // 有効な方向が見つからなかった場合は
        // ステージ内に収まるよう座標を補正
        Vector3 fallbackDirection =
            (player.position -
             stageBoundary.transform.position).normalized;

        Vector3 fallbackPosition =
            player.position - fallbackDirection * spawnDistance;

        return ClampInsideStage(fallbackPosition);
    }

    private bool IsInsideStage(Vector3 position)
    {
        Vector3 local =
            stageBoundary.transform.InverseTransformPoint(
                position
            );

        float limitX = Mathf.Max(
            0f,
            stageBoundary.HalfWidth - boundaryMargin
        );

        float limitZ = Mathf.Max(
            0f,
            stageBoundary.HalfDepth - boundaryMargin
        );

        return
            Mathf.Abs(local.x) <= limitX &&
            Mathf.Abs(local.z) <= limitZ;
    }

    private Vector3 ClampInsideStage(Vector3 position)
    {
        Vector3 local =
            stageBoundary.transform.InverseTransformPoint(
                position
            );

        float limitX = Mathf.Max(
            0f,
            stageBoundary.HalfWidth - boundaryMargin
        );

        float limitZ = Mathf.Max(
            0f,
            stageBoundary.HalfDepth - boundaryMargin
        );

        local.x = Mathf.Clamp(
            local.x, -limitX, limitX
        );

        local.z = Mathf.Clamp(
            local.z, -limitZ, limitZ
        );

        return stageBoundary.transform.TransformPoint(local);
    }
}
