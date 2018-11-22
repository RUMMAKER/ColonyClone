using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SetSelectedUnitState : BaseAction {
    public override string ActionName { get; set; }
    public override string ActionDesc { get; set; }
    public UnitState state;

    public SetSelectedUnitState(UnitState s)
    {
        state = s;
    }

    public override void DoAction()
    {
        LockStepManager.singleton.AddPendingAction(new LockStepSetUnitState(state, 
            PlayerManager.singleton.selectedUnits.Select(x => x.UnitId).ToArray()));
    }
}