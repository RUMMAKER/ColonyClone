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
        public int playerId;
        public List<List<ILockStepAction>> clientActions;
        public ClientInfo()
        {
            playerId = -1;
            clientActions = new List<List<ILockStepAction>>();
        }
    }
    Dictionary<int, ClientInfo> clientInfo = new Dictionary<int, ClientInfo>();

    public static MyNetworkManager singleton = null;
    public bool isAtStartup = true;
    NetworkClient myClient;

    // seed is syncronized across all client/server to ensure determinism.
    public int seed;
    

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
            if (NetworkServer.active && Input.GetKeyDown(KeyCode.S))
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
            if (NetworkServer.active)
            {
                GUI.Label(new Rect(2, 10, 150, 100), "Press S to start game");
            }
        }
    }
    public void SetupServer()
    {
        // Initialize seed value.
        seed = new System.Random().Next();
        // Register message handlers.
        NetworkServer.RegisterHandler(MsgType.Connect, ServerOnConnected);
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
        seed = netMsg.ReadMessage<IntegerMessage>().value;
        SceneManager.singleton.rng = new System.Random(seed);
        Debug.Log("CLIENT: Receive seed " + seed + " from server.");
    }
    public void ClientOnAction(NetworkMessage netMsg)
    {
        // On receive action from server, add action to lockstepmanager's action list.
        MyMsgActions msg = netMsg.ReadMessage<MyMsgActions>();
        byte[] objAsBytes = msg.serializedObj;
        ILockStepAction[] actions = DeSerializeActionsArr(objAsBytes);
        LockStepManager.singleton.AddLockStepActions(new List<ILockStepAction>(actions));
    }
    public void ClientOnGameStart(NetworkMessage netMsg)
    {
        Debug.Log("CLIENT: Receive game start msg from server.");
        SceneManager.singleton.gameStarted = true;
        // Send 2 empty lockstep action to server.
        // This is VERY important because this will build a buffer of lockstep actions.
        // If this is not done, lockstepmanager will NEVER send any actions to server because
        // lockstepmanager will only send actions AFTER successfully executing an action it receive from server.
        ClientSendActions(new List<ILockStepAction>());
        ClientSendActions(new List<ILockStepAction>());
    }
    public void ServerOnConnected(NetworkMessage netMsg)
    {
        Debug.Log("SERVER: Client " + netMsg.conn.connectionId + " connected.");
        // Keep track of clientInfo object for the new connection.
        clientInfo[netMsg.conn.connectionId] = new ClientInfo();
        // Send seed value to new connection to sync seed value.
        netMsg.conn.Send((short)MyMsgType.Seed, new IntegerMessage(seed));
    }
    public void ServerOnAction(NetworkMessage netMsg)
    {
        // Add client action to clientInfo.
        MyMsgActions msg = netMsg.ReadMessage<MyMsgActions>();
        byte[] objAsBytes = msg.serializedObj;
        ILockStepAction[] actions = DeSerializeActionsArr(objAsBytes);
        clientInfo[netMsg.conn.connectionId].clientActions.Add(new List<ILockStepAction>(actions));

        // Check if all clients have sent actions.
        foreach (NetworkConnection conn in NetworkServer.connections)
        {
            if (clientInfo[conn.connectionId].clientActions.Count == 0) return;
        }

        // If so, dequeue action from all clients and send to all clients.
        List<ILockStepAction> allClientActions = new List<ILockStepAction>();
        foreach (NetworkConnection conn in NetworkServer.connections)
        {
            allClientActions.AddRange(clientInfo[conn.connectionId].clientActions[0]);
            clientInfo[conn.connectionId].clientActions.RemoveAt(0);
        }
        MyMsgActions msgSend = new MyMsgActions();
        msgSend.serializedObj = SerializeActionsArr(allClientActions.ToArray());
        NetworkServer.SendToAll((short)MyMsgType.Actions, msgSend);
    }

    public void ClientSendActions(List<ILockStepAction> actions)
    {
        // Send actions to server.
        ILockStepAction[] actionsToSend = actions.ToArray();
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
    private byte[] SerializeActionsArr(ILockStepAction[] actions)
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, actions); // Should put in try-catch.
        byte[] serializedObj = stream.ToArray();
        stream.Close();
        return serializedObj;
    }
    private ILockStepAction[] DeSerializeActionsArr(byte[] actions)
    {
        ILockStepAction[] deserializedActions = null;
        MemoryStream stream = new MemoryStream();
        stream.Write(actions, 0, actions.Length);
        stream.Seek(0, SeekOrigin.Begin);
        BinaryFormatter formatter = new BinaryFormatter();
        deserializedActions = (ILockStepAction[])formatter.Deserialize(stream);
        stream.Close();
        return deserializedActions;
    }
}