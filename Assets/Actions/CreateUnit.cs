using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
    }

    public MyMsgType GetMsgType()
    {
        return MyMsgType.CreateUnit;
    }

    public MessageBase GetMsgBase()
    {
        MyMsgCreateUnit msg = new MyMsgCreateUnit();
        msg.playerId = playerId;
        msg.unitType = (int)type;
        return msg;
    }

    public void DebugLog()
    {
        Debug.Log("Msg parsed");
    }
}
