using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Mirror;
using UnityEngine;

public class ServerStatus
{
    public string ip;
    // not all transports use a port. assume default port. feel free to also send a port if needed.
    //public ushort port;
    public string title;
    public ushort players;
    public ushort capacity;

    public bool isLocal = false;

    public int lastLatency = -1;
#if !UNITY_WEBGL
    // Ping isn't known in WebGL builds
    public Ping ping;
#endif
    public ServerStatus(string ip, /*ushort port,*/ string title, ushort players, ushort capacity,
        bool isLocal = false)
    {
        this.ip = ip;
        //this.port = port;
        this.title = title;
        this.players = players;
        this.capacity = capacity;
        this.isLocal = isLocal;
#if !UNITY_WEBGL
        // Ping isn't known in WebGL builds
        ping = new Ping(ip);
#endif
    }
}

[RequireComponent(typeof(NetworkManager))]
public class ListServer : MonoBehaviour
{
    [Header("List Server Connection")]
    public string listServerIp = "127.0.0.1";
    public ushort gameServerToListenPort = 8887;
    public ushort clientToListenPort = 8888;

    Telepathy.Client gameServerToListenConnection = new Telepathy.Client(16 * 1024); //TODO: This message size is just a guess based on TelepathyTransport.clientMaxMessageSize to make it compile, check if this is a sensible value
    Telepathy.Client clientToListenConnection = new Telepathy.Client(16 * 1024); //TODO: This message size is just a guess based on TelepathyTransport.clientMaxMessageSize to make it compile, check if this is a sensible value

    // all the servers, stored as dict with unique ip key so we can
    // update them more easily
    // (use "ip:port" if port is needed)
    public Dictionary<string, ServerStatus> list = new Dictionary<string, ServerStatus>();

    public void ConnectToListServer()
    {
        // cannot use InvokeRepeating(nameof(Tick), 0, 1); because affected by Time Scale
        StartCoroutine(InvokeRealtimeTickCoroutine(1));
    }
    
    private IEnumerator InvokeRealtimeTickCoroutine(float seconds)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(seconds);
            Tick();
        }
    }

    bool IsConnecting() => NetworkClient.active && !ClientScene.ready;
    bool FullyConnected() => NetworkClient.active && ClientScene.ready;

    // should we use the client to listen connection?
    bool UseClientToListen()
    {

#if UNITY_SERVER
        return false;
#endif

        return !NetworkServer.active && !FullyConnected();
        //return !NetworkManager.isHeadless && !NetworkServer.active && !FullyConnected();
    }

    // should we use the game server to listen connection?
    bool UseGameServerToListen()
    {
        return NetworkServer.active;
    }

    void Tick()
    {
        TickGameServer();
        TickClient();
    }

    // send server status to list server
    void SendStatus()
    {
        BinaryWriter writer = new BinaryWriter(new MemoryStream());

        // create message
        writer.Write((ushort)NetworkServer.connections.Count);
        writer.Write((ushort)Maroon.NetworkManager.Instance.maxConnections);
        byte[] titleBytes = Encoding.UTF8.GetBytes(Maroon.NetworkManager.Instance.ServerName);
        writer.Write((ushort)titleBytes.Length);
        writer.Write(titleBytes);
        writer.Flush();

        // list server only allows up to 128 bytes per message
        if (writer.BaseStream.Position <= 128)
        {
            // send it
            System.ArraySegment<byte> send_it_message = new System.ArraySegment<byte>(((MemoryStream)writer.BaseStream).ToArray());
            gameServerToListenConnection.Send(send_it_message);
        }
        else
        {
            // TODO: Use logger
            Debug.Log("[List Server] List Server will reject messages longer than 128 bytes. Please use a shorter title.");
        } 
    }

    void TickGameServer()
    {
        // send server data to listen
        if (UseGameServerToListen())
        {
            // connected yet?
            if (gameServerToListenConnection.Connected)
            {
                if(Maroon.NetworkManager.Instance.PortsMapped)
                    SendStatus();
            }
            // otherwise try to connect
            // (we may have just started the game)
            else if (!gameServerToListenConnection.Connecting)
            {
                // TODO: Use logger
                Debug.Log("[List Server] GameServer connecting......");
                gameServerToListenConnection.Connect(listServerIp, gameServerToListenPort);
            }
        }
        // shouldn't use game server, but still using it?
        else if (gameServerToListenConnection.Connected)
        {
            gameServerToListenConnection.Disconnect();
        }
    }

    void ParseMessage(byte[] bytes)
    {
        // note: we don't use ReadString here because the list server
        //       doesn't know C#'s '7-bit-length + utf8' encoding for strings
        BinaryReader reader = new BinaryReader(new MemoryStream(bytes, false), Encoding.UTF8);
        byte ipBytesLength = reader.ReadByte();
        byte[] ipBytes = reader.ReadBytes(ipBytesLength);
        string ip = new IPAddress(ipBytes).ToString();
        //ushort port = reader.ReadUInt16(); <- not all Transports use a port. assume default.
        ushort players = reader.ReadUInt16();
        ushort capacity = reader.ReadUInt16();
        ushort titleLength = reader.ReadUInt16();
        string title = Encoding.UTF8.GetString(reader.ReadBytes(titleLength));
        //logger.Log("PARSED: ip=" + ip + /*" port=" + port +*/ " players=" + players + " capacity= " + capacity + " title=" + title);

        // build key
        string key = ip/* + ":" + port*/;

        // find existing or create new one
        if (list.TryGetValue(key, out ServerStatus server))
        {
            // refresh
            server.title = title;
            server.players = players;
            server.capacity = capacity;
        }
        else
        {
            // create
            server = new ServerStatus(ip, /*port,*/ title, players, capacity);
        }

        // save
        list[key] = server;
    }

    void TickClient()
    {
        // receive client data from listen
        if (UseClientToListen())
        {
            // connected yet?
            if (clientToListenConnection.Connected)
            {
                // receive latest game server info
                // TODO: The transport does not support this function anymore, need to find another way or rewrite the whole list server or use websockets or something
                // TODO: HEEEEEEEEEEEEEEEEEEEEEEEEERE

                /*
                while (clientToListenConnection.GetNextMessage(out Telepathy.Message message))
                {
                    // connected?
                    if (message.eventType == Telepathy.EventType.Connected)
                    {
                        // TODO: Use logger
                        Debug.Log("[List Server] Client connected!");
                    }

                    // data message?
                    else if (message.eventType == Telepathy.EventType.Data)
                    {
                        ParseMessage(message.data);
                    }

                    // disconnected?
                    else if (message.eventType == Telepathy.EventType.Disconnected)
                    {
                        // TODO: Use logger
                        Debug.Log("[List Server] Client disconnected.");
                    }
                }
                */

#if !UNITY_WEBGL
                // Ping isn't known in WebGL builds
                // ping again if previous ping finished
                foreach (ServerStatus server in list.Values)
                {
                    if (server.ping.isDone)
                    {
                        server.lastLatency = server.ping.time;
                        server.ping = new Ping(server.ip);
                    }
                }
#endif
            }
            // otherwise try to connect
            // (we may have just joined the menu/disconnect from game server)
            else if (!clientToListenConnection.Connecting)
            {
                // TODO: Use logger
                Debug.Log("[List Server] Client connecting...");

                clientToListenConnection.Connect(listServerIp, clientToListenPort);
            }
        }
        // shouldn't use client, but still using it? (e.g. after joining)
        else if (clientToListenConnection.Connected)
        {
            clientToListenConnection.Disconnect();
            list.Clear();
        }
    }

    public bool GetListServerStatus()
    {
        return clientToListenConnection.Connected || gameServerToListenConnection.Connected;
    }

    // disconnect everything when pressing Stop in the Editor
    void OnApplicationQuit()
    {
        if (gameServerToListenConnection.Connected)
            gameServerToListenConnection.Disconnect();
        if (clientToListenConnection.Connected)
            clientToListenConnection.Disconnect();
    }
}
