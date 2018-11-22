using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitForwardButton : Button {
    public BaseAction clickAction = new SetSelectedUnitState(UnitState.Forward);
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        clickAction.DoAction();
        Debug.Log("Click Forward");
    }
}