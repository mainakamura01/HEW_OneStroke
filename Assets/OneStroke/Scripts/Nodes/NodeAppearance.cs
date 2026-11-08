
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Node))]
public class NodeAppearance : MonoBehaviour
{
    [Header("Appearance Settings")]
    [SerializeField] private float visibleDuration = 10f;
    [SerializeField] private float minHiddenDuration = 1f;
    [SerializeField] private float maxHiddenDuration = 5f;

    [Header("Animation Settings")]
    [SerializeField] private float appearDuration = 0.3f;
    [SerializeField] private float disappearDuration = 0.3f;
    [SerializeField] private float overshootScale = 1.2f;

    [Header("Spawn Reference")]
    [SerializeField] private NodeSpawnManager nodeSpawnManager;

    private Node node;
    private Renderer[] renderers;
    private Coroutine appearanceCoroutine;

    private Vector3 originalScale;

    public bool IsVisible { get; private set; }

    private void Awake()
    {
        node = GetComponent<Node>();

        Transform visual = transform.Find("NodeVisual");

        if (visual != null)
        {
            renderers = visual.GetComponentsInChildren<Renderer>(true);
        }
        else
        {
            renderers = GetComponentsInChildren<Renderer>();
        }

        originalScale = transform.localScale;
    }

    private void Start()
    {
        appearanceCoroutine = StartCoroutine(AppearanceLoop());
    }

    private IEnumerator AppearanceLoop()
    {
        while (true)
        {
            // 選択済みなら表示を維持
            if (node.IsActivated)
            {
                SetVisible(true);
                transform.localScale = originalScale;

                yield return null;
                continue;
            }

            // 消失アニメーション
            yield return AnimateDisappear();

            // 非表示の待機時間
            float hiddenTime = Random.Range(
                minHiddenDuration,
                maxHiddenDuration
            );


            yield return new WaitForSeconds(hiddenTime);

            // 非表示の間に新しい位置へ移動
            if (!node.IsActivated && nodeSpawnManager != null)
            {
                nodeSpawnManager.TryMoveNode(this);
            }

            if (node.IsActivated)
            {
                SetVisible(true);
                transform.localScale = originalScale;
                continue;
            }

            // 出現アニメーション
            yield return AnimateAppear();


            // 表示時間
            float timer = 0f;

            while (timer < visibleDuration)
            {
                if (node.IsActivated)
                    break;

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator AnimateAppear()
    {
        SetVisible(true);

        float elapsed = 0f;

        while (elapsed < appearDuration)
        {
            if (node.IsActivated)
            {
                transform.localScale = originalScale;
                yield break;
            }

            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, appearDuration)
            );

            // 途中で少し大きくなって戻る
            float scale;

            if (t < 0.7f)
            {
                scale = Mathf.Lerp(
                    0f,
                    overshootScale,
                    t / 0.7f
                );
            }
            else
            {
                scale = Mathf.Lerp(
                    overshootScale,
                    1f,
                    (t - 0.7f) / 0.3f
                );
            }

            transform.localScale = originalScale * scale;

            yield return null;
        }

        transform.localScale = originalScale;
    }

    private IEnumerator AnimateDisappear()
    {
        if (!IsVisible)
        {
            transform.localScale = Vector3.zero;
            yield break;
        }

        float elapsed = 0f;
        Vector3 startScale = transform.localScale;

        while (elapsed < disappearDuration)
        {
            // 選択されたら消失を中止
            if (node.IsActivated)
            {
                transform.localScale = originalScale;
                SetVisible(true);
                yield break;
            }

            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, disappearDuration)
            );

            transform.localScale = Vector3.Lerp(
                startScale,
                Vector3.zero,
                t
            );

            yield return null;
        }

        transform.localScale = Vector3.zero;
        SetVisible(false);
    }

    private void SetVisible(bool visible)
    {
        IsVisible = visible;

        foreach (Renderer nodeRenderer in renderers)
        {
            nodeRenderer.enabled = visible;
        }
    }

    public void RestartVisibility()
    {
        if (appearanceCoroutine != null)
        {
            StopCoroutine(appearanceCoroutine);
        }

        transform.localScale = originalScale;
        SetVisible(true);

        appearanceCoroutine = StartCoroutine(
            RestartVisibilityRoutine()
        );
    }

    private IEnumerator RestartVisibilityRoutine()
    {
        float timer = 0f;

        while (timer < visibleDuration)
        {
            if (node.IsActivated)
            {
                // 選択されたら表示を維持する
                while (node.IsActivated)
                    yield return null;

                // 再び青色に戻ったら表示時間を再スタート
                timer = 0f;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        appearanceCoroutine = StartCoroutine(AppearanceLoop());
    }


}
