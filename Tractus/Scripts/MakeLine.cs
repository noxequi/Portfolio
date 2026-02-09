using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MakeLine : MonoBehaviour
{
    public float minDistance = 0.01f;
    private float maxLength = 7f;
    public GameObject linePrefab;

    private LineRenderer currentLineRenderer;
    private List<Vector3> currentPoints = new List<Vector3>();
    private float currentDrawingLength = 0f;
    private List<LineInfo> allLines = new List<LineInfo>();
    private float totalLength = 0f;

    private class LineInfo
    {
        public GameObject lineObject;
        public LineRenderer renderer;
        public EdgeCollider2D collider;
        public List<Vector3> points;
        public float length;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartNewLine();
        }

        if (Input.GetMouseButton(0) && currentLineRenderer != null)
        {
            DrawLine();
        }

        if (Input.GetMouseButtonUp(0) && currentLineRenderer != null)
        {
            EndLine();
        }

        if (!canDraw)
        {
            return;
        }
    }

    void StartNewLine()
    {
        GameObject newLineObj = Instantiate(linePrefab);
        currentLineRenderer = newLineObj.GetComponent<LineRenderer>();

        currentLineRenderer.positionCount = 0;
        currentLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        currentLineRenderer.startColor = Color.white;
        currentLineRenderer.endColor = Color.white;
        currentLineRenderer.startWidth = 0.1f;
        currentLineRenderer.endWidth = 0.1f;
        currentLineRenderer.useWorldSpace = true;

        currentPoints.Clear();
        currentDrawingLength = 0f;
    }

    void DrawLine()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        if (currentPoints.Count == 0 || Vector3.Distance(currentPoints.Last(), mousePosition) > minDistance)
        {
            float segmentLength = currentPoints.Count > 0 ? Vector3.Distance(currentPoints.Last(), mousePosition) : 0f;
            currentPoints.Add(mousePosition);
            currentDrawingLength += segmentLength;
            totalLength += segmentLength;

            while (currentDrawingLength > maxLength && currentPoints.Count >= 2)
            {
                float lengthToRemove = Vector3.Distance(currentPoints[0], currentPoints[1]);

                currentDrawingLength -= lengthToRemove;
                totalLength -= lengthToRemove;

                currentPoints.RemoveAt(0);
            }

            TrimOldLines();
            UpdateCurrentLineRenderer();
        }
    }

    void EndLine()
    {
        if (currentPoints.Count >= 2)
        {
            GameObject lineObj = currentLineRenderer.gameObject;
            EdgeCollider2D edgeCollider = lineObj.AddComponent<EdgeCollider2D>();
            edgeCollider.SetPoints(currentPoints.Select(p => (Vector2)p).ToList());

            lineObj.layer = LayerMask.NameToLayer("Line");

            allLines.Add(new LineInfo
            {
                lineObject = lineObj,
                renderer = currentLineRenderer,
                collider = edgeCollider,
                points = new List<Vector3>(currentPoints),
                length = currentDrawingLength
            });
        }
        else
        {
            if (currentLineRenderer != null)
            {
                Destroy(currentLineRenderer.gameObject);
            }
        }

        currentLineRenderer = null;
        currentPoints.Clear();
    }

    void UpdateCurrentLineRenderer()
    {
        if (currentLineRenderer != null && currentPoints != null)
        {
            currentLineRenderer.positionCount = currentPoints.Count;
            currentLineRenderer.SetPositions(currentPoints.ToArray());
        }
    }

    void TrimOldLines()
    {
        while (totalLength > maxLength && allLines.Count > 0)
        {
            LineInfo oldestLine = allLines.First();

            if (oldestLine.points.Count < 2)
            {
                totalLength -= oldestLine.length;
                Destroy(oldestLine.lineObject);
                allLines.RemoveAt(0);
                continue;
            }

            float segmentLength = Vector3.Distance(oldestLine.points[0], oldestLine.points[1]);

            totalLength -= segmentLength;
            oldestLine.length -= segmentLength;

            oldestLine.points.RemoveAt(0);

            if (oldestLine.points.Count < 2)
            {
                Destroy(oldestLine.lineObject);
                allLines.RemoveAt(0);
            }
            else
            {
                oldestLine.renderer.SetPositions(oldestLine.points.ToArray());
                oldestLine.collider.SetPoints(oldestLine.points.Select(p => (Vector2)p).ToList());
            }
        }
    }

    public bool HasAnyPoints()
    {
        return currentPoints.Count > 0;
    }

    public Vector2 GetLastPoint()
    {
        if (currentPoints.Count == 0)
            return Vector2.zero;
        return currentPoints[currentPoints.Count - 1];
    }

    private bool canDraw = true;
    public void EnableDrawing(bool enable)
    {
        canDraw = enable;
    }

}