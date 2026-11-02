
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Node))]
public class NodeAppearance : MonoBehaviour
{
    [Header("Appearance Settings")]
    [SerializeField] private float visibleDuration = 10f;
    [SerializeField] private float minHiddenDuration = 1f;
    [SerializeField] private float maxHiddenDuration = 5f;

    private Node node;
    private Renderer[] renderers;
    private Coroutine appearanceCoroutine;

    public bool IsVisible { get; private set; }

    private void Awake()
    {
        node = GetComponent<Node>();
        renderers = GetComponentsInChildren<Renderer>();
    }

    private void Start()
    {
        appearanceCoroutine = StartCoroutine(AppearanceLoop());
    }

    private IEnumerator AppearanceLoop()
    {
        while (true)
        {
            // 起動済みなら表示状態を維持
            if (node.IsActivated)
            {
                SetVisible(true);
                yield return null;
                continue;
            }

            // ランダムな時間だけ非表示
            SetVisible(false);

            float hiddenTime = Random.Range(
                minHiddenDuration,
                maxHiddenDuration
            );

            yield return new WaitForSeconds(hiddenTime);

            // 青いNodeを表示
            SetVisible(true);

            // 10秒間表示する
            float timer = 0f;

            while (timer < visibleDuration)
            {
                if (node.IsActivated)
                    break;

                timer += Time.deltaTime;
                yield return null;
            }

            // 起動済みなら次のループで固定表示
        }
    }

    private void SetVisible(bool visible)
    {
        IsVisible = visible;

        foreach (Renderer nodeRenderer in renderers)
        {
            nodeRenderer.enabled = visible;
        }
    }
}
