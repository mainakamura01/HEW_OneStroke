
using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform player;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnDistance = 8f;

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
        if (enemyPrefab == null || player == null)
        {
            Debug.LogError(
                "EnemySpawner: Enemy Prefab or Player is missing!"
            );
            yield break;
        }

        while (true)
        {
            // 通常ウェーブ
            isEnhancedWave = false;
            Debug.Log("通常ウェーブ開始！");

            // 生存中の敵が4体になるまで補充
            SpawnUntilCount(normalEnemyCount);

            float elapsed = 0f;

            while (elapsed < normalDuration)
            {
                yield return new WaitForSeconds(normalCheckInterval);

                elapsed += normalCheckInterval;

                if (elapsed < normalDuration)
                {
                    SpawnUntilCount(normalEnemyCount);
                }
            }

            // 強化ウェーブ
            isEnhancedWave = true;
            Debug.Log("強化ウェーブ開始！");

            // 生存中の敵が12体になるまで追加
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
            Debug.Log("強化ウェーブ終了！通常に戻ります");
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
            float angle =
                startAngle + (360f / count) * i;

            float radians = angle * Mathf.Deg2Rad;

            Vector3 direction = new Vector3(
                Mathf.Cos(radians),
                0f,
                Mathf.Sin(radians)
            );

            Vector3 spawnPosition =
                player.position + direction * spawnDistance;

            Instantiate(
                enemyPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        Debug.Log(
            $"{count}体の敵を出現させました！" +
            $" 現在のウェーブ: {(isEnhancedWave ? "強化" : "通常")}"
        );
    }
}
