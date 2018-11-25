using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox {
    public int height;
    public int width;
    public Point relativePosition;

    public HitBox(int width, int height, Point relativePosition)
    {
        this.height = height;
        this.width = width;
        this.relativePosition = relativePosition;
    }

    public void DebugDrawHitbox(Point p)
    {
        Vector3 topleft = new Vector3((float)(p.x + relativePosition.x - width / 2) / SceneManager.integerScale,
            (float)(p.y + relativePosition.y + height / 2) / SceneManager.integerScale, 0);
        topleft = Camera.main.WorldToScreenPoint(topleft);

        Vector3 botRight = new Vector3((float)(p.x + relativePosition.x + width / 2) / SceneManager.integerScale,
            (float)(p.y + relativePosition.y - height / 2) / SceneManager.integerScale, 0);
        botRight = Camera.main.WorldToScreenPoint(botRight);

        DrawScreenRect(GetScreenRect(topleft, botRight));
    }
    private void DrawScreenRect(Rect rect)
    {
        // Draw borders.
        float borderThickness = 2f;
        GUI.color = Color.green;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, borderThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, borderThickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - borderThickness, rect.yMin, borderThickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - borderThickness, rect.width, borderThickness), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
