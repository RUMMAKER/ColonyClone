using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Marine : Unit {
    override protected void Awake()
    {
        base.Awake();
        speed = 3;
        selectableName = "Marine";
        selectableDesc = "Expendable anti-air infantry.";
        hitboxes = new HitBox[1];
        hitboxes[0] = new HitBox(40, 80, new Point(0,0));
    }
}