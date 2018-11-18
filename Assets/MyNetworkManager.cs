using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MyNetworkManager : MonoBehaviour
{
    public static MyNetworkManager singleton = null;
    public bool isAtStartup = true;
    NetworkClient myClient;

    void Awake()
    {
        if (singleton != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        singleton = this;
    }

    void Update()
    {
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                SetupServer();
                SetupClient();
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetupClient();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ServerSendTestMsg();
            }
        }
    }
    void OnGUI()
    {
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 10, 150, 100), "Press H to host");
            GUI.Label(new Rect(2, 30, 150, 100), "Press C to connect");
        }
    }
    public void SetupServer()
    {
        NetworkServer.Listen(4444);
        isAtStartup = false;
    }
    public void SetupClient()
    {
        if (!NetworkServer.active) myClient = new NetworkClient();
        myClient = ClientScene.ConnectLocalServer();
        
        // Register message handlers.
        myClient.RegisterHandler(MsgType.Connect, OnConnected);
        myClient.RegisterHandler((short)MyMsgType.CreateUnit, OnTestMsg);

        if (!NetworkServer.active) myClient.Connect("127.0.0.1", 4444);
        isAtStartup = false;
    }

    // Message Handlers.
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server");
    }
    public void OnTestMsg(NetworkMessage netMsg)
    {
        IAction action = MyMsgReader.ReadMsg(netMsg);
        action.DebugLog();
    }

    public void ServerSendTestMsg()
    {
        if (!NetworkServer.active) Debug.Log("You are client");
        CreateUnit action = new CreateUnit(UnitType.Marine, 0);
        MessageBase baseMsg = action.GetMsgBase();
        MyMsgType msgType = action.GetMsgType();
        NetworkServer.SendToAll((short)msgType, baseMsg);
    }

    public void ClientSendLockStepActions(List<IAction> actions)
    {
        if (myClient == null) Debug.Log("client is null");
        // do stuff
    }
}
