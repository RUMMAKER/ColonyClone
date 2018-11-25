using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour {

    private Vector3 mousePosition1;
    private bool isSelecting = false;
    private Color colorInner = new Color(0.8f, 0.8f, 0.9f, 0.25f);
    private Color colorBorder = new Color(0.8f, 0.8f, 0.9f, 1f);
    private BoxCollider2D boxCollider;
    private ContactFilter2D filter = new ContactFilter2D();
    private Collider2D[] inBox = new Collider2D[1000];
    
    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        filter = filter.NoFilter();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!GUIManager.singleton.mouseOverUI)
            {
                SelectRaycast(Input.mousePosition);
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                SelectInBox(mousePosition1, Input.mousePosition);
            }
        } 
    }
    void OnGUI()
    {
        if (isSelecting)
        {
            DrawScreenRect(GetScreenRect(mousePosition1, Input.mousePosition), 1f, colorInner, colorBorder);
        }
    }

    private void SelectInBox(Vector3 mousePosition1, Vector3 mousePosition2)
    {
        isSelecting = false;
        if (mousePosition1.Equals(mousePosition2))
        {
            return;
        }

        // Change boxCollider size and position.
        Vector3 corner1 = Camera.main.ScreenToWorldPoint(mousePosition1);
        Vector3 corner2 = Camera.main.ScreenToWorldPoint(mousePosition2);
        float width = System.Math.Abs(corner1.x - corner2.x);
        float height = System.Math.Abs(corner1.y - corner2.y);
        Vector3 center = new Vector3((corner1.x + corner2.x) / 2f, (corner1.y + corner2.y) / 2f, 0);
        boxCollider.size = new Vector2(width, height);
        gameObject.transform.position = center;

        // Select UNITS in box.
        System.Array.Clear(inBox, 0, inBox.Length);
        int inBoxCount = boxCollider.OverlapCollider(filter, inBox);
        List<SelectableBehaviour> playerUnitsInBox = new List<SelectableBehaviour>();
        for (int i = 0; i < inBoxCount; i++)
        {
            SelectableBehaviour s = inBox[i].gameObject.GetComponent<Unit>();
            if (s != null && s.playerId == PlayerManager.singleton.playerId)
            {
                playerUnitsInBox.Add(s);
            }
        }
        if (playerUnitsInBox.Count > 0)
        {
            GUIManager.singleton.DeselectAll();
            foreach (SelectableBehaviour s in playerUnitsInBox)
            {
                GUIManager.singleton.Select(s);
            }
        }
        GUIManager.singleton.UpdateButtons();
    }
    private void SelectRaycast(Vector2 mousePosition)
    {
        isSelecting = true;
        mousePosition1 = mousePosition;
        // Select selectable from raycast.
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), 
            Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Default"));
        if (hit.collider != null)
        {
            GUIManager.singleton.DeselectAll();
            GUIManager.singleton.Select(hit.collider.gameObject.GetComponent<SelectableBehaviour>());
        }
        GUIManager.singleton.UpdateButtons();
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
