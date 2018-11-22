using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

	public void OnPointerEnter(PointerEventData p)
    {
        GUIManager.singleton.mouseOverUI = true;
    }

    public void OnPointerExit(PointerEventData p)
    {
        GUIManager.singleton.mouseOverUI = false;
    }
}
