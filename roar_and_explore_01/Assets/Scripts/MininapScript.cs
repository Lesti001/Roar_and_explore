using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public Camera mainCamera;

    private Rect minimapRect = new Rect(Screen.width - 310, 10, 300, 300);

    private const float worldMinX = -39f;
    private const float worldMaxX = 39f;
    private const float worldMinY = -4f;
    private const float worldMaxY = 34.5f;

    private Dictionary<Color, Texture2D> colorTextures = new Dictionary<Color, Texture2D>();

    private float cameraFrameWorldWidth = 18f;
    private float cameraAspect = 16f / 9f;
    private float cameraHeightMultiplier = 0.85f;

    private Color backgroundColor = new Color32(215, 191, 139, 255);
    private Color borderColor = new Color32(75, 65, 35, 255);

    private void OnGUI()
    {
        GUI.color = backgroundColor;
        GUI.DrawTexture(minimapRect, Texture2D.whiteTexture);
        GUI.color = Color.white;

        DrawCameraFrame();

        foreach (var road in TourManagerScript.instance.getRoads())
        {
            for (int i = 0; i < road.Count - 1; i++)
            {
                if (road[i] != null && road[i + 1] != null)
                {
                    Vector2 start = WorldToMinimap(road[i]);
                    Vector2 end = WorldToMinimap(road[i + 1]);
                    DrawLine(start, end, Color.white, 4f);
                }
            }
        }

        foreach (var animal in AnimalManager.Instance.getChipedAnimals())
        {
            if (animal != null)
                DrawDot(WorldToMinimap(animal.transform.position), Color.red, 8);
        }

        foreach (var food in AnimalManager.Instance.getFoodSources())
        {
            DrawDot(WorldToMinimap(food.Item1), Color.green, 8);
        }

        foreach (var water in AnimalManager.Instance.getWaterSources())
        {
            DrawDot(WorldToMinimap(water), Color.blue, 8);
        }

        foreach (var obj in AnimalManager.Instance.getRangers())//TODO RANGET LIST
        {
            if (obj != null)
                DrawDot(WorldToMinimap(obj.transform.position), Color.black, 8);
        }
        foreach (var obj in AnimalManager.Instance.GetJeeps())
        {
            if (obj != null)
                DrawDot(WorldToMinimap(obj.transform.position), Color.grey, 10);
        }
        

        GUI.DrawTexture(new Rect(minimapRect.x, minimapRect.y, minimapRect.width, 2), GetTexture(borderColor));
        GUI.DrawTexture(new Rect(minimapRect.x, minimapRect.yMax - 2, minimapRect.width, 2), GetTexture(borderColor));
        GUI.DrawTexture(new Rect(minimapRect.x, minimapRect.y, 2, minimapRect.height), GetTexture(borderColor));
        GUI.DrawTexture(new Rect(minimapRect.xMax - 2, minimapRect.y, 2, minimapRect.height), GetTexture(borderColor));

        if (Event.current.type == EventType.MouseDown && minimapRect.Contains(Event.current.mousePosition))
        {
            Vector2 mousePos = Event.current.mousePosition;
            Vector2 worldTarget = MinimapToWorld(mousePos);
            Vector3 cameraTarget = new Vector3(worldTarget.x, worldTarget.y, mainCamera.transform.position.z);
            mainCamera.transform.position = cameraTarget;
        }
    }

    private Vector2 WorldToMinimap(Vector2 worldPos)
    {
        float normalizedX = Mathf.InverseLerp(worldMinX, worldMaxX, worldPos.x);
        float normalizedY = Mathf.InverseLerp(worldMinY, worldMaxY, worldPos.y);

        float x = minimapRect.x + normalizedX * minimapRect.width;
        float y = minimapRect.y + (1 - normalizedY) * minimapRect.height;
        return new Vector2(x, y);
    }

    private Vector2 MinimapToWorld(Vector2 minimapPos)
    {
        float normalizedX = (minimapPos.x - minimapRect.x) / minimapRect.width;
        float normalizedY = 1 - ((minimapPos.y - minimapRect.y) / minimapRect.height);

        float worldX = Mathf.Lerp(worldMinX, worldMaxX, normalizedX);
        float worldY = Mathf.Lerp(worldMinY, worldMaxY, normalizedY);
        return new Vector2(worldX, worldY);
    }

    private void DrawDot(Vector2 position, Color color, float size)
    {
        Texture2D tex = GetTexture(color);
        GUI.DrawTexture(new Rect(position.x - size / 2, position.y - size / 2, size, size), tex);
    }

    private void DrawCameraFrame()
    {
        Vector2 center = WorldToMinimap(mainCamera.transform.position);

        float worldHeight = (cameraFrameWorldWidth / cameraAspect) * cameraHeightMultiplier;

        float normWidth = cameraFrameWorldWidth / (worldMaxX - worldMinX);
        float normHeight = worldHeight / (worldMaxY - worldMinY);

        float width = normWidth * minimapRect.width;
        float height = normHeight * minimapRect.height;

        float x = center.x - width / 2f;
        float y = center.y - height / 2f;

        Texture2D tex = GetTexture(Color.red);

        GUI.DrawTexture(new Rect(x, y, width, 2), tex);
        GUI.DrawTexture(new Rect(x, y + height - 2, width, 2), tex);
        GUI.DrawTexture(new Rect(x, y, 2, height), tex);
        GUI.DrawTexture(new Rect(x + width - 2, y, 2, height), tex);
    }

    private Texture2D GetTexture(Color color)
    {
        if (!colorTextures.TryGetValue(color, out Texture2D tex))
        {
            tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, color);
            tex.Apply();
            colorTextures[color] = tex;
        }
        return tex;
    }
    private void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
    {
        Matrix4x4 matrix = GUI.matrix;
        Color savedColor = GUI.color;

        Vector2 delta = pointB - pointA;
        float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        float length = delta.magnitude;

        GUI.color = color;
        GUIUtility.RotateAroundPivot(angle, pointA);
        GUI.DrawTexture(new Rect(pointA.x, pointA.y, length, width), GetTexture(color));
        GUI.matrix = matrix;
        GUI.color = savedColor;
    }

}
