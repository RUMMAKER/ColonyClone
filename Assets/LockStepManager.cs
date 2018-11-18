using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStepManager : MonoBehaviour {

    private class LockStepQueue
    {
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

        public int Count()
        {
            return lockStepQueue.Count;
        }
    }

    public static LockStepManager singleton = null;
    private LockStepQueue lockStepQueue;
    private bool lockStepSuccess = false;

    private float timeSinceLastGameUpdate = 0f;
    private int gameUpdateCount = 0;
    private readonly float gameUpdateFrequency = 0.05f; // 20 GameUpdate per second
    private readonly int gameUpdatePerLockstep = 4; // 0.25 LockStepUpdate per GameUpdate

    void Awake()
    {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
        lockStepQueue = new LockStepQueue();
    }

    void Update () {
        timeSinceLastGameUpdate += Time.deltaTime;
        if (timeSinceLastGameUpdate >= gameUpdateFrequency)
        {
            GameUpdate();
        }
	}

    void GameUpdate()
    {
        if (gameUpdateCount == gameUpdatePerLockstep)
        {
            lockStepSuccess = LockStepUpdate();
            gameUpdateCount -= 1;
        }
        if (lockStepSuccess)
        {
            foreach (IHasGameUpdate obj in SceneManager.singleton.gameUpdateObjs)
            {
                obj.GameUpdate();
            }
        }
        gameUpdateCount++;
        timeSinceLastGameUpdate -= gameUpdateFrequency;
    }

    bool LockStepUpdate()
    {
        List<IAction> nextStep = lockStepQueue.Pop();
        if (nextStep == null) return false;

        // Send Actions to server.
        MyNetworkManager.singleton.ClientSendLockStepActions(SceneManager.singleton.actions);
        SceneManager.singleton.actions.Clear();

        foreach (IAction action in nextStep)
        {
            action.DoAction();
        }
        return true;
    }
}
