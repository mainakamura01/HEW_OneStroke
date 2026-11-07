
using System.Collections;
using TMPro;
using UnityEngine;

public class InvalidShapeUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject warningRoot;
    [SerializeField] private TMP_Text messageText;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 1.5f;

    private Coroutine hideCoroutine;

    private void Awake()
    {
        if (warningRoot != null)
            warningRoot.SetActive(false);
    }

    public void ShowWarning(string message)
    {
        if (warningRoot == null)
            return;

        if (messageText != null)
            messageText.text = message;

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        warningRoot.SetActive(true);

        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSecondsRealtime(displayDuration);

        if (warningRoot != null)
            warningRoot.SetActive(false);

        hideCoroutine = null;
    }
}
