using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum MyMsgType : short
{
    CreateUnit = 1000
}

public class MyMsgCreateUnit : MessageBase
{
    public int playerId;
    public int unitType;
}

// Reads NetworkMessage and return the corresponding IAction.
public class MyMsgReader
{
    public static IAction ReadMsg(NetworkMessage netMsg)
    {
        MyMsgType type = (MyMsgType)netMsg.msgType;
        switch (type)
        {
            case MyMsgType.CreateUnit:
                MyMsgCreateUnit msg = netMsg.ReadMessage<MyMsgCreateUnit>();
                return new CreateUnit((UnitType)msg.unitType, msg.playerId);
        }
        return null;
    }
}