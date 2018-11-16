using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStepManager : MonoBehaviour {

    private List<IHasGameFrame> gameFrameObj;
    private List<IAction> actionsToSend;

    private LockStepQueue lockStepQueue;
    private bool lockStepSuccess = false;

    private float accumuilatedTime = 0f;
    private readonly float frameLength = 0.05f; // 20 GameUpdate per second
    private readonly int gameUpdatePerLockstep = 4; // 0.25 LockStepUpdate per GameUpdate
    private int gameUpdateCount = 0;

    void Start()
    {
        lockStepQueue = new LockStepQueue();
        gameFrameObj = new List<IHasGameFrame>();
        actionsToSend = new List<IAction>();
    }

    void Update () {
        accumuilatedTime += Time.deltaTime;
        if (accumuilatedTime >= frameLength)
        {
            GameUpdate();
            accumuilatedTime -= frameLength;
        }
	}

    void GameUpdate()
    {
        if (gameUpdateCount == gameUpdatePerLockstep)
        {
            lockStepSuccess = LockStepUpdate();
            gameUpdateCount = 0;
        }
        if (lockStepSuccess)
        {
            foreach (IHasGameFrame obj in gameFrameObj)
            {
                obj.GameUpdate();
            }
            // NetworkManager.SendActions(actionsToSend);
            // actionsToSend.Clear();
        }
        gameUpdateCount++;
    }

    bool LockStepUpdate()
    {
        List<IAction> nextStep = lockStepQueue.Pop();
        if (nextStep == null)
        {
            return false;
        }
        foreach (IAction action in nextStep)
        {
            action.DoAction();
        }
        return true;
    }

    public void AddGameFrameObj(IHasGameFrame obj)
    {
        gameFrameObj.Add(obj);
    }

    public void RemoveGameFrameObj(IHasGameFrame obj)
    {
        gameFrameObj.Remove(obj);
    }

    public void AddAction(IAction a)
    {
        // Could optimize by not adding action if it is a redundent action (spam clicking same action).
        actionsToSend.Add(a);
    }

    // AddLockStep should be called by the NetworkManager when it receives the nextLockStep from server
    public void AddLockStep(List<IAction> actions)
    {
        lockStepQueue.Push(actions);
    }
}
