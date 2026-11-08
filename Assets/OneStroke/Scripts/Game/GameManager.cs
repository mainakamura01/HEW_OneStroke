
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("References")]
    [SerializeField] private ScoreManager scoreManager;

    private bool isGameEnded = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public void EndGame(string message)
    {
        if (isGameEnded)
            return;

        isGameEnded = true;

        if (resultText != null)
        {
            resultText.text = message;

            if (message == "GAME CLEAR!")
            {
                // ゲームクリア：ゴールド
                resultText.color = new Color32(255, 226, 140, 255);
            }
            else
            {
                // ゲームオーバー：ピンク
                resultText.color = new Color32(255, 150, 165, 255);
            }
        }

        if (finalScoreText != null &&
            scoreManager != null)
        {
            finalScoreText.text =
                $"FINAL SCORE : {scoreManager.CurrentScore:000000}";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0f;

        Debug.Log("Game Ended: " + message);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void ReturnToTitle()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("TitleScene");
    }
}
