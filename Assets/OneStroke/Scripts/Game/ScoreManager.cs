
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Settings")]
    [SerializeField] private int pointsPerEnemy = 100;

    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;

    private int currentScore = 0;

    public int CurrentScore => currentScore;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddEnemyScore()
    {
        currentScore += pointsPerEnemy;

        UpdateScoreUI();

        Debug.Log($"Score: {currentScore}");
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text =
                $"SCORE\n{currentScore:000000}";
        }
    }
}
