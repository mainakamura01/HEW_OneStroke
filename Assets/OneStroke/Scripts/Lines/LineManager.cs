
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lineWidth = 0.12f;
    [SerializeField] private float lineHeight = 0.15f;

    [Header("Completed Line")]
    [SerializeField] private Material dimLineMaterial;

    [Header("Completion Flash")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.25f;
    [SerializeField] private float flashWidthMultiplier = 3f;
    [SerializeField]
    private Color flashColor =
        new Color(0.7f, 0.95f, 1f, 1f);

    private readonly List<GameObject> lines = new();

    private Coroutine flashCoroutine;
    private GameObject flashRoot;

    [Header("Traveling Light")]
    [SerializeField] private float travelDuration = 0.2f;
    [SerializeField] private float travelWidth = 0.28f;
    [SerializeField] private float travelLength = 0.8f;

    private Coroutine travelCoroutine;
    private GameObject travelObject;

    [Header("Double Glow")]
    [SerializeField] private Material travelOuterMaterial;
    [SerializeField] private Material travelCoreMaterial;
    [SerializeField] private float outerWidth = 0.9f;
    [SerializeField] private float coreWidth = 0.25f;


    public void PlayTravelingLight()
    {
        if (travelCoroutine != null)
        {
            StopCoroutine(travelCoroutine);
            travelCoroutine = null;
        }

        if (travelObject != null)
        {
            Destroy(travelObject);
            travelObject = null;
        }

        travelCoroutine = StartCoroutine(TravelingLightRoutine());
    }


    private IEnumerator TravelingLightRoutine()
    {
        // ノードを選んだ順番で線の座標を集める
        List<Vector3> points = new();

        foreach (GameObject lineObject in lines)
        {
            if (lineObject == null)
                continue;

            LineRenderer line =
                lineObject.GetComponent<LineRenderer>();

            if (line == null)
                continue;

            if (points.Count == 0)
                points.Add(line.GetPosition(0));

            points.Add(line.GetPosition(1));
        }

        if (points.Count < 2)
        {
            travelCoroutine = null;
            yield break;
        }

        List<float> lengths = new();
        List<float> distances = new() { 0f };

        float totalLength = 0f;

        for (int i = 0; i < points.Count - 1; i++)
        {
            float length = Vector3.Distance(
                points[i], points[i + 1]
            );

            lengths.Add(length);
            totalLength += length;
            distances.Add(totalLength);
        }

        if (totalLength <= 0.001f)
        {
            travelCoroutine = null;
            yield break;
        }

        travelObject = new GameObject("TravelingLight");
        travelObject.transform.SetParent(transform);


        LineRenderer outerLine =
            CreateTravelRenderer(
                "OuterGlow",
                travelOuterMaterial,
                outerWidth
            );

        LineRenderer coreLine =
            CreateTravelRenderer(
                "WhiteCore",
                travelCoreMaterial,
                coreWidth
            );


        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, travelDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float headDistance = totalLength * t;

            float tailDistance = Mathf.Max(
                0f, headDistance - travelLength
            );

            // 光の先端と後端
            List<Vector3> lightPoints = new();

            lightPoints.Add(GetPointAtDistance(
                points, lengths, tailDistance
            ));

            // 先端と後端の間にある角をすべて追加
            for (int i = 1; i < points.Count - 1; i++)
            {
                if (distances[i] > tailDistance &&
                    distances[i] < headDistance)
                {
                    lightPoints.Add(points[i]);
                }
            }

            lightPoints.Add(GetPointAtDistance(
                points, lengths, headDistance
            ));

            // 通常の線より少し上に表示
            for (int i = 0; i < lightPoints.Count; i++)
            {
                Vector3 p = lightPoints[i];
                p.y += 0.05f;
                lightPoints[i] = p;
            }

            Vector3[] positions = lightPoints.ToArray();

            outerLine.positionCount = positions.Length;
            outerLine.SetPositions(positions);

            coreLine.positionCount = positions.Length;
            coreLine.SetPositions(positions);

            yield return null;
        }

        if (travelObject != null)
        {
            Destroy(travelObject);
            travelObject = null;
        }

        travelCoroutine = null;
    }


    private LineRenderer CreateTravelRenderer(
        string objectName,
        Material material,
        float width)
    {
        GameObject obj = new GameObject(objectName);
        obj.transform.SetParent(travelObject.transform);

        LineRenderer line = obj.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;

        line.startWidth = width;
        line.endWidth = width;

        line.numCapVertices = 8;
        line.numCornerVertices = 8;

        if (material != null)
            line.sharedMaterial = material;

        line.startColor = Color.white;
        line.endColor = Color.white;

        line.shadowCastingMode =
            UnityEngine.Rendering.ShadowCastingMode.Off;

        line.receiveShadows = false;

        return line;
    }


    private Vector3 GetPointAtDistance(
        List<Vector3> points,
        List<float> lengths,
        float distance)
    {
        float traveled = 0f;

        for (int i = 0; i < lengths.Count; i++)
        {
            float segmentLength = lengths[i];

            if (distance <= traveled + segmentLength)
            {
                float t = Mathf.Clamp01(
                    (distance - traveled) /
                    Mathf.Max(0.001f, segmentLength)
                );

                return Vector3.Lerp(
                    points[i],
                    points[i + 1],
                    t
                );
            }

            traveled += segmentLength;
        }

        return points[points.Count - 1];
    }


    public void CreateLine(Node startNode, Node endNode)
    {
        if (travelCoroutine != null)
        {
            StopCoroutine(travelCoroutine);
            travelCoroutine = null;
        }

        if (travelObject != null)
        {
            Destroy(travelObject);
            travelObject = null;
        }

        if (startNode == null || endNode == null)
            return;

        GameObject lineObject = new GameObject(
            $"Line_{startNode.name}_{endNode.name}"
        );

        lineObject.transform.SetParent(transform);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();

        line.useWorldSpace = true;
        line.positionCount = 2;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.numCapVertices = 4;
        line.numCornerVertices = 4;

        if (lineMaterial != null)
            line.sharedMaterial = lineMaterial;

        Vector3 startPosition = startNode.transform.position;
        Vector3 endPosition = endNode.transform.position;

        startPosition.y = lineHeight;
        endPosition.y = lineHeight;

        line.SetPosition(0, startPosition);
        line.SetPosition(1, endPosition);

        lines.Add(lineObject);

        Debug.Log(
            $"Line created: {startNode.name} -> {endNode.name}"
        );
    }

    public void PlayCompletionFlash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (flashRoot != null)
            Destroy(flashRoot);

        flashCoroutine = StartCoroutine(CompletionFlash());
    }

    private IEnumerator CompletionFlash()
    {
        flashRoot = new GameObject("CompletionFlash");
        flashRoot.transform.SetParent(transform);

        List<LineRenderer> flashLines = new();

        foreach (GameObject lineObject in lines)
        {
            if (lineObject == null)
                continue;

            LineRenderer original =
                lineObject.GetComponent<LineRenderer>();

            if (original == null)
                continue;

            GameObject flashObject =
                new GameObject("FlashLine");

            flashObject.transform.SetParent(flashRoot.transform);

            LineRenderer flash =
                flashObject.AddComponent<LineRenderer>();

            flash.useWorldSpace = true;
            flash.positionCount = 2;
            flash.numCapVertices = 6;
            flash.numCornerVertices = 6;

            flash.startWidth = lineWidth * flashWidthMultiplier;
            flash.endWidth = lineWidth * flashWidthMultiplier;

            if (flashMaterial != null)
                flash.sharedMaterial = flashMaterial;

            Vector3 start = original.GetPosition(0);
            Vector3 end = original.GetPosition(1);

            // 通常の線より少し上に重ねる
            start.y += 0.02f;
            end.y += 0.02f;

            flash.SetPosition(0, start);
            flash.SetPosition(1, end);

            flash.startColor = flashColor;
            flash.endColor = flashColor;

            flashLines.Add(flash);
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, flashDuration);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float width = Mathf.Lerp(
                lineWidth * flashWidthMultiplier,
                lineWidth,
                t
            );

            foreach (LineRenderer flash in flashLines)
            {
                if (flash == null)
                    continue;

                flash.startWidth = width;
                flash.endWidth = width;
            }

            yield return null;
        }

        if (flashRoot != null)
        {
            Destroy(flashRoot);
            flashRoot = null;
        }

        flashCoroutine = null;
    }

    public void RemoveLastLine()
    {
        if (lines.Count == 0)
            return;

        int lastIndex = lines.Count - 1;
        GameObject lastLine = lines[lastIndex];

        lines.RemoveAt(lastIndex);

        if (lastLine != null)
            Destroy(lastLine);
    }

    public void ClearLines()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (flashRoot != null)
        {
            Destroy(flashRoot);
            flashRoot = null;
        }

        foreach (GameObject line in lines)
        {
            if (line != null)
                Destroy(line);
        }

        lines.Clear();
    }


    public void DimCompletedLines()
    {
        if (dimLineMaterial == null)
            return;

        foreach (GameObject lineObject in lines)
        {
            if (lineObject == null)
                continue;

            LineRenderer line =
                lineObject.GetComponent<LineRenderer>();

            if (line != null)
                line.sharedMaterial = dimLineMaterial;
        }
    }

}
