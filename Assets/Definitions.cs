using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;

#region enums and interface
public enum UnitType
{
    Marine, Tank
}
public enum UnitState
{
    Initial, Forward, Hold, Back, Charge
}
public interface IHasGameUpdate
{
    void GameUpdate();
}
public interface IAction
{
    void DoAction();
}
#endregion

// Label for actions(buttons).
public class UIAction
{
    public string labelName;
    public string labelDesc;
    public Sprite labelImg;
    public IAction action;

    public UIAction(string name, string desc, Sprite img, IAction action)
    {
        labelName = name;
        labelDesc = desc;
        labelImg = img;
        this.action = action;
    }
}
public class Point
{
    public int x;
    public int y;
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public void Add(Point p)
    {
        x += p.x;
        y += p.y;
    }
    private float ToFloat(int i)
    {
        return i / (float)SceneManager.integerScale;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(ToFloat(x), ToFloat(y), ToFloat(y));
    }
}
public abstract class SelectableBehaviour : MonoBehaviour
{
    public int playerId;
    public int selectableId;
    public string selectableName;
    public string selectableDesc;
    protected bool selected;

    public virtual void OnSelect()
    {
        selected = true;
    }
    public virtual void OnDeSelect()
    {
        selected = false;
    }

    public virtual UIAction[] GetActions()
    {
        return new UIAction[0];
    }

    public virtual float GetProgress()
    {
        return 0f;
    }
}

#region actions
[System.Serializable]
public class ActionSpawnUnit : IAction
{
    public int playerId;
    public UnitType type;

    public ActionSpawnUnit(UnitType t, int playerId)
    {
        this.playerId = playerId;
        type = t;
    }

    public void DoAction()
    {
        SpawnManager.singleton.SpawnUnit(type, playerId);
    }
}
[System.Serializable]
public class ActionSetUnitState : IAction
{
    public int[] unitIds;
    public UnitState state;

    public ActionSetUnitState(UnitState s, int[] unitIds)
    {
        this.unitIds = unitIds;
        state = s;
    }

    public void DoAction()
    {
        foreach (int id in unitIds)
        {
            Unit u = SceneManager.singleton.units[id];
            if (u != null) u.state = state;
        }
    }
}
[System.Serializable]
public class ActionSetBuildingState : IAction
{
    public int buildingId;

    public int taskTime;
    public IAction doOnComplete;
    public bool repeat;

    public ActionSetBuildingState(int buildingId, int taskTime, IAction doOnComplete, bool repeat)
    {
        this.buildingId = buildingId;
        this.taskTime = taskTime;
        this.doOnComplete = doOnComplete;
        this.repeat = repeat;
    }

    public void DoAction()
    {
        Building b = SceneManager.singleton.buildings[buildingId];
        b.StartTask(taskTime, doOnComplete, repeat);
    }
}
[System.Serializable]
public class ActionCancel : IAction
{
    public int buildingId;

    public ActionCancel(int buildingId)
    {
        this.buildingId = buildingId;
    }

    public void DoAction()
    {
        Building b = SceneManager.singleton.buildings[buildingId];
        b.CancelTask();
    }
}
#endregion

#region network messages
public enum MyMsgType : short
{
    Actions = 1000,
    Seed = 1001,
    PlayerId = 1002,
    StartGame = 1003
}

public class MyMsgActions : MessageBase
{
    public byte[] serializedObj;
}
#endregion