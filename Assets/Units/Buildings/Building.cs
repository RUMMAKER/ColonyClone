using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : SelectableBehaviour, IHasGameUpdate {

    private int timer;
    private int taskTime;
    private bool doingTask;
    private bool repeat;
    private IAction doOnTaskFinish;

    public void Start()
    {
        selectableName = "Test building";
        selectableDesc = "Test building desc.";
        SceneManager.singleton.buildings.Add(selectableId, this);
    }
    public void StartTask(int taskTime, IAction doOnComplete, bool repeat)
    {
        this.taskTime = taskTime;
        doingTask = true;
        timer = 0;
        doOnTaskFinish = doOnComplete;
        this.repeat = repeat;
    }
    public void CancelTask()
    {
        doingTask = false;
        timer = 0;
    }
    public void GameUpdate()
    {
        if (doingTask)
        {
            timer += 1;
            if (timer >= taskTime)
            {
                doOnTaskFinish.DoAction();
                doingTask = false;
                if (repeat)
                {
                    // if action is not cancel
                    StartTask(taskTime, doOnTaskFinish, true);
                }
            }
        }
    }
    public override UIAction[] GetActions()
    {
        UIAction[] actions = new UIAction[2];
        actions[0] = new UIAction("Build Marine",
            "Marines are expendable anti-air infantry.",
            ResourceManager.singleton.chargeImg,
            new ActionSetBuildingState(selectableId, 50, new ActionSpawnUnit(UnitType.Marine, playerId), true));
        actions[1] = new UIAction("Cancel",
            "Cancel current action.",
            ResourceManager.singleton.chargeImg,
            new ActionCancel(selectableId));
        return actions;
    }
    public override float GetProgress()
    {
        return (float)timer / taskTime;
    }
}
