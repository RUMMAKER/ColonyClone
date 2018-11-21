using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction {
    public abstract string ActionName { get; set; }
    public abstract string ActionDesc { get; set; }
    public abstract void DoAction();
}
