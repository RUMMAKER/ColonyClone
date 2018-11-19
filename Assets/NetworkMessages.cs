using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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