using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class MyNetworkManager : MonoBehaviour
{
    private class ClientInfo {
        public string playerName;
        public int playerId;
        public List<List<IAction>> clientActions = new List<List<IAction>>();
    }
    Dictionary<int, ClientInfo> clientInfo = new Dictionary<int, ClientInfo>();

    public static MyNetworkManager singleton;
    NetworkClient myClient;
    public bool isAtStartup = true;

    void Awake()
    {
        if (singleton != null)
        {
            Destroy(gameObject);
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
            if (NetworkServer.active && Input.GetKeyDown(KeyCode.S) && !SceneManager.singleton.gameStarted)
            {
                ServerSendGameStart();
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
        else
        {
            if (NetworkServer.active && !SceneManager.singleton.gameStarted)
            {
                GUI.Label(new Rect(2, 10, 150, 100), "Press S to start game");
            }
        }
    }
    public void SetupServer()
    {
        // Initialize seed value.
        SceneManager.singleton.seed = new System.Random().Next();
        // Register message handlers.
        NetworkServer.RegisterHandler(MsgType.Connect, ServerOnConnected);
        NetworkServer.RegisterHandler(MsgType.Disconnect, ServerOnDisconnect);
        NetworkServer.RegisterHandler((short)MyMsgType.Actions, ServerOnAction);
        NetworkServer.Listen(4444);
        isAtStartup = false;
    }
    public void SetupClient()
    {
        if (!NetworkServer.active)
        {
            myClient = new NetworkClient();
        }
        else
        {
            myClient = ClientScene.ConnectLocalServer();
        }
        // Register message handlers.
        myClient.RegisterHandler(MsgType.Connect, ClientOnConnected);
        myClient.RegisterHandler((short)MyMsgType.Seed, ClientOnSeed);
        myClient.RegisterHandler((short)MyMsgType.Actions, ClientOnAction);
        myClient.RegisterHandler((short)MyMsgType.StartGame, ClientOnGameStart);
        if (!NetworkServer.active) myClient.Connect("127.0.0.1", 4444);
        isAtStartup = false;
    }

    // Message Handlers.
    public void ClientOnConnected(NetworkMessage netMsg)
    {
        Debug.Log("CLIENT: Connected to server.");
    }  
    public void ClientOnSeed(NetworkMessage netMsg)
    {
        // On receive seed from server, set seed to same value.
        SceneManager.singleton.seed = netMsg.ReadMessage<IntegerMessage>().value;
        SceneManager.singleton.rng = new System.Random(SceneManager.singleton.seed);
        Debug.Log("CLIENT: Receive seed " + SceneManager.singleton.seed + " from server.");
    }
    public void ClientOnAction(NetworkMessage netMsg)
    {
        // On receive action from server, add action to lockstepmanager's action list.
        MyMsgActions msg = netMsg.ReadMessage<MyMsgActions>();
        byte[] objAsBytes = msg.serializedObj;
        IAction[] actions = DeSerializeActionsArr(objAsBytes);
        LockStepManager.singleton.AddConfirmedActions(new List<IAction>(actions));
    }
    public void ClientOnGameStart(NetworkMessage netMsg)
    {
        Debug.Log("CLIENT: Receive game start msg from server.");
        SceneManager.singleton.gameStarted = true;
        // Send 2 empty lockstep action to server.
        // This is VERY important because this will build a buffer of lockstep actions.
        // If this is not done, lockstepmanager will NEVER send any actions to server because
        // lockstepmanager will only send actions AFTER successfully executing an action it receive from server.
        ClientSendActions(new List<IAction>());
        ClientSendActions(new List<IAction>());
    }
    public void ServerOnDisconnect(NetworkMessage netMsg)
    {
        Debug.Log("SERVER: Client " + netMsg.conn.connectionId + " disconnected.");
        clientInfo.Remove(netMsg.conn.connectionId);

        // Check if all clients have sent actions.
        foreach (KeyValuePair<int, ClientInfo> c in clientInfo)
        {
            if (c.Value.clientActions.Count == 0) return;
        }

        // If so, dequeue action from all clients and send to all clients.
        List<IAction> allClientActions = new List<IAction>();
        foreach (KeyValuePair<int, ClientInfo> c in clientInfo)
        {
            allClientActions.AddRange(c.Value.clientActions[0]);
            c.Value.clientActions.RemoveAt(0);
        }
        MyMsgActions msgSend = new MyMsgActions();
        msgSend.serializedObj = SerializeActionsArr(allClientActions.ToArray());
        NetworkServer.SendToAll((short)MyMsgType.Actions, msgSend);
    }
    public void ServerOnConnected(NetworkMessage netMsg)
    {
        Debug.Log("SERVER: Client " + netMsg.conn.connectionId + " connected.");
        // Keep track of clientInfo object for the new connection.
        clientInfo[netMsg.conn.connectionId] = new ClientInfo();
        // Send seed value to new connection to sync seed value.
        netMsg.conn.Send((short)MyMsgType.Seed, new IntegerMessage(SceneManager.singleton.seed));
    }
    public void ServerOnAction(NetworkMessage netMsg)
    {
        // Add client action to clientInfo.
        MyMsgActions msg = netMsg.ReadMessage<MyMsgActions>();
        byte[] objAsBytes = msg.serializedObj;
        IAction[] actions = DeSerializeActionsArr(objAsBytes);
        clientInfo[netMsg.conn.connectionId].clientActions.Add(new List<IAction>(actions));

        // Check if all clients have sent actions.
        foreach (KeyValuePair<int, ClientInfo> c in clientInfo)
        {
            if (c.Value.clientActions.Count == 0) return;
        }

        // If so, dequeue action from all clients and send to all clients.
        List<IAction> allClientActions = new List<IAction>();
        foreach (KeyValuePair<int, ClientInfo> c in clientInfo)
        {
            allClientActions.AddRange(c.Value.clientActions[0]);
            c.Value.clientActions.RemoveAt(0);
        }
        MyMsgActions msgSend = new MyMsgActions();
        msgSend.serializedObj = SerializeActionsArr(allClientActions.ToArray());
        NetworkServer.SendToAll((short)MyMsgType.Actions, msgSend);
    }

    public void ClientSendActions(List<IAction> actions)
    {
        // Send actions to server.
        IAction[] actionsToSend = actions.ToArray();
        MyMsgActions msg = new MyMsgActions();
        msg.serializedObj = SerializeActionsArr(actionsToSend);
        myClient.Send((short)MyMsgType.Actions, msg);
    }
    public void ServerSendGameStart()
    {
        // Send game start to all clients.
        Debug.Log("SERVER: Send game start msg to clients.");
        NetworkServer.SendToAll((short)MyMsgType.StartGame, new EmptyMessage());
    }

    // Serializer helpers.
    private byte[] SerializeActionsArr(IAction[] actions)
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        try
        {
            formatter.Serialize(stream, actions);
        }
        catch
        {
            Debug.Log("Serialization failed.");
        }
        
        byte[] serializedObj = stream.ToArray();
        stream.Close();
        return serializedObj;
    }
    private IAction[] DeSerializeActionsArr(byte[] actions)
    {
        IAction[] deserializedActions = null;
        MemoryStream stream = new MemoryStream();
        stream.Write(actions, 0, actions.Length);
        stream.Seek(0, SeekOrigin.Begin);
        BinaryFormatter formatter = new BinaryFormatter();
        deserializedActions = (IAction[])formatter.Deserialize(stream);
        stream.Close();
        return deserializedActions;
    }
}