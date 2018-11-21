using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Unit : MonoBehaviour, IHasGameUpdate, ISelectable
{
    public abstract int PlayerId { get; set; }
    public abstract int UnitId { get; set; }
    public abstract Point SpawnPosition { get; set; }
    public abstract Point Position { get; set; }
    public abstract UnitState State { get; set; }
    public abstract void GameUpdate();
    public BaseAction[] GetActions()
    {
        return new BaseAction[0]; // WIP
    }
}
