using UnityEngine;
using UnityEngine.Networking;

public interface IHasGameUpdate
{
    void GameUpdate();
}

public interface IAction
{
    void DoAction();
}

public enum UnitType
{
    Marine, Tank
}

public enum UnitState
{
    Initial, Forward, Hold, Back, Charge
}