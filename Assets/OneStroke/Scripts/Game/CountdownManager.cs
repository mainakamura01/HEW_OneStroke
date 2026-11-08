using System.Collections;
using UnityEngine;
using TMPro;

[DefaultExecutionOrder(-1000)]
public class CountdownManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject countdownOverlay;

    [Header("Countdown Settings")]
    [SerializeField] private float goDisplayTime = 0.7f;

    public static bool IsCountingDown { get; private set; }

    private void Awake()
    {
        IsCountingDown = true;

        // ƒQ[ƒ€‚ÌŽžŠÔ‚ð’âŽ~
        Time.timeScale = 0f;
    }

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        if (countdownText == null)
        {
            Debug.LogWarning("CountdownText‚ªÝ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñB");

            FinishCountdown();
            yield break;
        }

        if (countdownOverlay != null)
            countdownOverlay.SetActive(true);

        countdownText.gameObject.SetActive(true);

        for (int i = 3; i >= 1; i--)
        {
            countdownText.text = i.ToString();
            countdownText.color = Color.white;

            yield return new WaitForSecondsRealtime(1f);
        }

        countdownText.text = "GO!";
        countdownText.color = new Color32(255, 226, 140, 255);

        yield return new WaitForSecondsRealtime(goDisplayTime);

        FinishCountdown();
    }

    private void FinishCountdown()
    {
        if (countdownText != null)
            countdownText.gameObject.SetActive(false);

        if (countdownOverlay != null)
            countdownOverlay.SetActive(false);

        IsCountingDown = false;
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        IsCountingDown = false;
    }
}