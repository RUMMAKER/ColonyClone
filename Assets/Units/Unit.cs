using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IHasGameUpdate {
    abstract public int PlayerId { get; set; }
    abstract public int UnitId { get; set; }
    public abstract void GameUpdate();
}
