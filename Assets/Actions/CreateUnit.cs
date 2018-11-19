using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class CreateUnit : IAction {
    public int playerId;
    public UnitType type;

    public CreateUnit(UnitType t, int id)
    {
        playerId = id;
        type = t;
    }

	public void DoAction()
    {
        Debug.Log("CreateUnit: (playerId=" + playerId + ", type=" + type.ToString() + ")");
        SceneManager.singleton.SpawnMarine(playerId);
    }
}
