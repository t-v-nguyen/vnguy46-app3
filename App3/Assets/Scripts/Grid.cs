using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Grid<TGridObject>
{
    private int width;
    private int height;
    private float cellSize;
    Vector3 origin;
    private TGridObject[,] gridArray;
    private TextMesh[,] debugTextArray;
    public Grid(int width, int height, float cellSize, Vector3 origin)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        bool showDebug = true;
        if (showDebug)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    debugTextArray[x, y] = CreateTextMesh(gridArray[x, y]?.ToString(), Color.white, null, GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2), 20);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }
    public Grid(int width, int height, float cellSize, Vector3 origin, Func<Grid<TGridObject>, int, int, TGridObject> createGridObject)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new TGridObject[width, height];
        debugTextArray = new TextMesh[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridArray[x,y] = createGridObject(this, x, y);
            }
        }

        bool showDebug = false;
        if (showDebug)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    debugTextArray[x, y] = CreateTextMesh(gridArray[x, y]?.ToString(), Color.white, null, GetWorldPosition(x, y) + new Vector3(cellSize / 2, cellSize / 2), 20);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }
    }

    public int GetHeight()
    {
        return this.height;
    }

    public int GetWidth()
    {
        return this.width;
    }

    public float GetCellSize()
    {
        return this.cellSize;
    }

    public static TextMesh CreateTextMesh(string text, Color color, Transform parent = null, Vector3 localPosition = default(Vector3), int fontSize = 40, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left, int sortingOrder = 5000)
    {
        GameObject go = new GameObject("Grid_Outline", typeof(TextMesh));
        Transform transform = go.transform;
        transform.SetParent(parent);
        transform.localPosition = localPosition;
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
        return textMesh;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + origin;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - origin).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - origin).y / cellSize);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            gridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
    }

    public TGridObject GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        if (x >= 0 && y >= 0 && x <= width && y <= height)
        {
            return gridArray[x, y];
        }
        else
        {
            return default(TGridObject);
        }
    }

}
