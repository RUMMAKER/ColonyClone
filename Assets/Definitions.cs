using UnityEngine;
using UnityEngine.Networking;

public interface IHasGameUpdate
{
    void GameUpdate();
}
public interface ILockStepAction
{
    void DoLockStepAction();
}
public interface ISelectable
{
    BaseAction[] GetActions();
}
public enum UnitType
{
    Marine, Tank
}
public enum UnitState
{
    Initial, Forward, Hold, Back, Charge
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