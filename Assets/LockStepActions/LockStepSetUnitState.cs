using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class LockStepSetUnitState : ILockStepAction
{
    public int[] unitIds;
    public UnitState state;

    public LockStepSetUnitState(UnitState s, int[] ids)
    {
        unitIds = ids;
        state = s;
    }

    public void DoLockStepAction()
    {
        foreach (int id in unitIds)
        {
            Unit u = SceneManager.singleton.units[id];
            if (u != null)
            {
                u.State = state;
            }
        }
    }
}
