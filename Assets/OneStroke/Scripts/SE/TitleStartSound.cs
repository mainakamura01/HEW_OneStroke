
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleStartSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip startSound;

    [Header("Scene")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("Button")]
    [SerializeField] private Button startButton;

    private bool isStarting = false;

    public void OnStartButtonClicked()
    {
        if (isStarting)
            return;

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        isStarting = true;

        if (startButton != null)
            startButton.interactable = false;

        if (audioSource != null && startSound != null)
        {
            audioSource.PlayOneShot(startSound);

            // å¯â âπÇ™èIÇÌÇÈÇ‹Ç≈ë“Ç¬
            yield return new WaitForSecondsRealtime(
                startSound.length
            );
        }

        SceneManager.LoadScene(gameSceneName);
    }
}
