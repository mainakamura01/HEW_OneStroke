
using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float gameDuration = 360f;

    [Header("UI References")]
    [SerializeField] private TMP_Text timerText;

    private float remainingTime;
    private bool isTimerRunning;

    [SerializeField] private GameManager gameManager;
    private void Start()
    {
        remainingTime = Mathf.Max(0f, gameDuration);
        isTimerRunning = true;

        UpdateTimerUI();
    }

    private void Update()
    {
        // カウントダウン中はタイマーを進めない
        if (CountdownManager.IsCountingDown)
            return;

        if (!isTimerRunning)
            return;

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            isTimerRunning = false;

            Debug.Log("GAME CLEAR! 制限時間を生き残った！");

            if (gameManager != null)
            {
                gameManager.EndGame("GAME CLEAR!");
            }
        }

        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        // 画面には切り上げた残り秒数を表示
        int totalSeconds =
            Mathf.CeilToInt(remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text =
            $"{minutes:00}:{seconds:00}";
    }
}
