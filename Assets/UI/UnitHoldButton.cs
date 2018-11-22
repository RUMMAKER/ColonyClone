using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitHoldButton : Button
{
    public BaseAction clickAction = new SetSelectedUnitState(UnitState.Hold);
    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        clickAction.DoAction();
        Debug.Log("Click Hold");
    }
}