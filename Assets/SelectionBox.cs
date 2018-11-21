using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour {

    private Color colorInner = new Color(0.8f, 0.8f, 0.9f, 0.25f);
    private Color colorBorder = new Color(0.8f, 0.8f, 0.9f, 1f);
    private bool isSelecting = false;
    private Vector3 mousePosition1;
    [SerializeField]
    private GameObject physicalBox;
    private BoxCollider2D physicalBoxCollider;
    private ContactFilter2D filter = new ContactFilter2D();

     // testing only, move to GUI manager later

    void Awake()
    {
        physicalBoxCollider = physicalBox.GetComponent<BoxCollider2D>();
        filter = filter.NoFilter();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            mousePosition1 = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            GetCollidersInBox(mousePosition1, Input.mousePosition);
        } 
    }
    void OnGUI()
    {
        if (isSelecting)
        {
            DrawScreenRect(GetScreenRect(mousePosition1, Input.mousePosition), 1f, colorInner, colorBorder);
        }
    }

    private void GetCollidersInBox(Vector3 mousePosition1, Vector3 mousePosition2)
    {
        Vector3 corner1 = Camera.main.ScreenToWorldPoint(mousePosition1);
        Vector3 corner2 = Camera.main.ScreenToWorldPoint(mousePosition2);
        float width = System.Math.Abs(corner1.x - corner2.x);
        float height = System.Math.Abs(corner1.y - corner2.y);
        Vector3 center = new Vector3((corner1.x + corner2.x) / 2f, (corner1.y + corner2.y) / 2f, 0);
        physicalBoxCollider.size = new Vector2(width, height);
        physicalBox.transform.position = center;

        System.Array.Clear(GUIManager.singleton.selected, 0, GUIManager.singleton.selected.Length);
        int count = physicalBoxCollider.OverlapCollider(filter, GUIManager.singleton.selected);
        Debug.Log("SELECTED: " + count + " colliders.");
    }
    private Rect GetScreenRect(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
    private void DrawScreenRect(Rect rect, float borderThickness, Color boxColor, Color borderColor)
    {
        // Draw box.
        GUI.color = boxColor;
        GUI.DrawTexture(rect, Texture2D.whiteTexture);
        // Draw borders.
        GUI.color = borderColor;
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, borderThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, borderThickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMax - borderThickness, rect.yMin, borderThickness, rect.height), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - borderThickness, rect.width, borderThickness), Texture2D.whiteTexture);
        GUI.color = Color.white;
    }
}
