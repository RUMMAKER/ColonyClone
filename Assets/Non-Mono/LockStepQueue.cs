using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStepQueue {
    // Using List instead of Queue because List is serializable
    // lockStepQueue is list of instructions for each future lockStep update where instructions is a list of actions
    private List<List<IAction>> lockStepQueue;

    public LockStepQueue()
    {
        lockStepQueue = new List<List<IAction>>();
    }

    public void Push(List<IAction> lockStepStep)
    {
        lockStepQueue.Add(lockStepStep);
    }

    public List<IAction> Pop()
    {
        if (lockStepQueue.Count == 0) return null;
        List<IAction> returnable = lockStepQueue[0];
        lockStepQueue.RemoveAt(0);
        return returnable;
    }

    public int GetCount()
    {
        return lockStepQueue.Count;
    }
}
