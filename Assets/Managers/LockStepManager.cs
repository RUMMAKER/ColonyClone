using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockStepManager : MonoBehaviour {

    public static LockStepManager singleton = null;
    private List<IAction> pendingActions = new List<IAction>();
    private List<List<IAction>> confirmedActions = new List<List<IAction>>();
    private bool lockStepSuccess = false;

    private float timeSinceLastGameUpdate = 0f;
    private int gameUpdateCount = 0;
    private readonly float gameUpdateFrequency = 0.05f; // 20 GameUpdate per second
    private readonly int gameUpdatePerLockstep = 4; // 0.25 LockStepUpdate per GameUpdate

    void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        singleton = this;
    }
    void Update () {
        if (!SceneManager.singleton.gameStarted) return;

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
            foreach (KeyValuePair<int, Building> kv in SceneManager.singleton.buildings)
            {
                kv.Value.GameUpdate();
            }
            foreach (KeyValuePair<int, Unit> kv in SceneManager.singleton.units)
            {
                kv.Value.GameUpdate();
            }
        }
        gameUpdateCount++;
        timeSinceLastGameUpdate -= gameUpdateFrequency;
    }
    bool LockStepUpdate()
    {
        if (confirmedActions.Count == 0) return false;

        // Dequeue the next confirmedAction.
        List<IAction> nextStep = confirmedActions[0];
        confirmedActions.RemoveAt(0);
        // Send pendingActions to server.
        MyNetworkManager.singleton.ClientSendActions(pendingActions);
        pendingActions.Clear();

        foreach (IAction action in nextStep)
        {
            action.DoAction();
        }
        return true;
    }

    public void AddPendingAction(IAction action)
    {
        pendingActions.Add(action);
    }
    public void AddConfirmedActions(List<IAction> actions)
    {
        confirmedActions.Add(actions);
    }
}
