using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IHasGameUpdate {
    public int playerId;
    public int objectId;
    public abstract void GameUpdate();
}
