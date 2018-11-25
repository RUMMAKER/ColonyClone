using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour {

    public float maxScrollSpeed;
    public float scrollSpeed;
    public float scrollAccel;
    public float scrollDeccel;

	void Update () {
        if (!GUIManager.singleton.mouseOverUI && Input.mousePosition.x >= Screen.width * 0.9)
        {
            scrollSpeed += scrollAccel * Time.deltaTime;
        }
        if (!GUIManager.singleton.mouseOverUI && Input.mousePosition.x <= Screen.width * 0.1)
        {
            scrollSpeed -= scrollAccel * Time.deltaTime;
        }
        transform.Translate(Vector3.right * scrollSpeed, Space.World);

        if (scrollSpeed > 0)
        {
            scrollSpeed -= scrollDeccel * Time.deltaTime;
            if (scrollSpeed < 0) scrollSpeed = 0;
        }
        else if (scrollSpeed < 0)
        {
            scrollSpeed += scrollDeccel * Time.deltaTime;
            if (scrollSpeed > 0) scrollSpeed = 0;
        }

        KeepInBounds();
    }
    public void KeepInBounds()
    {
        int leftBorder = (-SceneManager.singleton.mapWidth / SceneManager.integerScale / 2);
        int rightBorder = (SceneManager.singleton.mapWidth / SceneManager.integerScale / 2);
        if (transform.position.x < leftBorder)
        {
            transform.position = new Vector3(leftBorder, transform.position.y, transform.position.z);
            scrollSpeed = 0f;
        }
        if (transform.position.x > rightBorder)
        {
            transform.position = new Vector3(rightBorder, transform.position.y, transform.position.z);
            scrollSpeed = 0f;
        }
    }
}
