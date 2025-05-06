using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    private List<GameObject> lines = new List<GameObject>();
    public GameObject gridParent;

    // 40 szeles es 20 magas racsot rajzol ki:
    [Header("Grid settings")]
    [SerializeField] private bool showGrid = false;
    [SerializeField] private float x_lowerBound = -38.01f;
    [SerializeField] private float x_upperBound = 38f;
    [SerializeField] private float y_lowerBound = -4f;
    [SerializeField] private float y_upperBound = 34.01f;
    [SerializeField] private float gridSize = 1.9f;

    [Header("Line settings")]
    [SerializeField] private float lineWidth = 0.075f;
    [SerializeField] private Color lineColor = new Color(0f, 0f, 0f, 0.5f);

    private void Start()
    {
        AddLine(x_lowerBound, y_lowerBound, x_lowerBound, y_upperBound, true);
        AddLine(x_lowerBound, y_lowerBound, x_upperBound, y_lowerBound, true);
        AddLine(x_upperBound, y_upperBound, x_lowerBound, y_upperBound, true);
        AddLine(x_upperBound, y_upperBound, x_upperBound, y_lowerBound, true);
    }

    public void ToggleGrid()
    {
        if (showGrid) ClearGrid();
        else CreateGrid();
        showGrid = !showGrid;
    }

    private void CreateGrid()
    {
        for (float i = x_lowerBound; i < x_upperBound; i += gridSize)
            AddLine(i, y_lowerBound, i, y_upperBound);
        for (float i = y_lowerBound; i < y_upperBound; i += gridSize)
            AddLine(x_lowerBound, i, x_upperBound, i);
    }

    private void AddLine(float x1, float y1, float x2, float y2, bool fence = false)
    {
        GameObject lineObj = new GameObject(fence ? "FenceLine" : "GridLine");
        if (!fence)
        {
            lineObj.transform.SetParent(gridParent.transform);
            lines.Add(lineObj);
        }

        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = (fence ? 0.1f : lineWidth);
        lr.endWidth = (fence ? 0.1f : lineWidth);
        lr.startColor = (fence ? new Color(75 / 255f, 65 / 255f, 35 / 255f, 1f) : lineColor);
        lr.endColor = (fence ? new Color(75 / 255f, 65 / 255f, 35 / 255f, 1f) : lineColor);
        lr.useWorldSpace = true;
        lr.positionCount = 2;
        lr.material = new Material(Shader.Find("Sprites/Default"));

        lr.SetPosition(0, new Vector3(x1, y1, 0));
        lr.SetPosition(1, new Vector3(x2, y2, 0));
    }

    private void ClearGrid()
    {
        foreach (GameObject obj in lines) Destroy(obj);
    }
}