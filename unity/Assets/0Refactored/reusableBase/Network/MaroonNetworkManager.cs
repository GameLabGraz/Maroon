using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GEAR.Localization;
using Maroon.UI;
using Mirror;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CharacterSpawnMessage : MessageBase
{
    public Vector3 CharacterPosition;
    public Quaternion CharacterRotation;
}

public class NetworkControlMessage : MessageBase
{
    public bool InControl;
}

public class ChangeSceneMessage : MessageBase
{
    public string SceneName;
}

public class MaroonNetworkManager : NetworkManager
{
    [HideInInspector]
    public static MaroonNetworkManager Instance = null;

    [Header("Maroon Network Manager")]
    [SerializeField] private GameObject preNetworkSyncVars;
    [SerializeField] private GameObject experimentPlayer;
    [Scene]
    [SerializeField] private List<string> networkEnabledExperiments;

    [HideInInspector]
    public UnityEvent onGetControl;
    [HideInInspector]
    public UnityEvent onLoseControl;
    
    private ListServer _listServer;
    private MaroonNetworkDiscovery _networkDiscovery;
    private PortForwarding _upnp;
    private GameManager _gameManager;
    private DialogueManager _dialogueManager;
    private NetworkSyncVariables _syncVariables;

    private bool _isStarted;

    public override void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }

    public override void Start()
    {
        base.Start();
        _listServer = GetComponent<ListServer>();
        _networkDiscovery = GetComponent<MaroonNetworkDiscovery>();
        _upnp = GetComponent<PortForwarding>();
        _gameManager = FindObjectOfType<GameManager>();

        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    public void StartMultiUser()
    {
        if(_isStarted)
            return;
        _listServer.ConnectToListServer();
        _networkDiscovery.StartDiscovery();
        _isStarted = true;
    }

    public bool IsActive()
    {
        return _isStarted;
    }

    private bool CheckSceneValid(string sceneName)
    {
        if (onlineScene.Contains(sceneName))
            return true;

        foreach (var enabledScene in networkEnabledExperiments)
        {
            if (enabledScene.Contains(sceneName))
                return true;
        }

        return false;
    }

    #region Server

    private bool _activePortMapping;
    private string _clientInControl;
    private Dictionary<string, NetworkConnection> _connectedPlayers = new Dictionary<string, NetworkConnection>();
    private Dictionary<NetworkConnection, NetworkPlayer> _connectedPlayerObjects = new Dictionary<NetworkConnection, NetworkPlayer>();
    
    public override void OnStartServer()
    {
        base.OnStartServer();
        
        MarkTakenServerNames();
        _serverName = NetworkNamingService.GetNewServerName();
        
        _networkDiscovery.AdvertiseServer();
        _upnp.SetupPortForwarding();
        NetworkServer.RegisterHandler<CharacterSpawnMessage>(OnCreateCharacter);
        NetworkServer.RegisterHandler<ChangeSceneMessage>(OnChangeSceneMessage);
        IsInControl = true;
        SpawnSyncVars();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        NetworkNamingService.FreeAllPlayerNames();
        NetworkNamingService.FreeAllServerNames();
        _activePortMapping = false;
        _clientInControl = null;
        _connectedPlayers.Clear();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (!SceneManager.GetActiveScene().name.Contains("Laboratory"))
        {
            //Connection only possible when in Laboratory
            conn.Disconnect();
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        base.OnServerDisconnect(conn);
        string disconnectedPlayerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        if (disconnectedPlayerName == null)
            return;
        if (disconnectedPlayerName == _clientInControl)
        {
            //Cannot use TakeControl because client already disconnected
            IsInControl = true;
            _clientInControl = _playerName;
        }
        _connectedPlayers.Remove(disconnectedPlayerName);
        NetworkNamingService.FreePlayerName(disconnectedPlayerName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        //For some strange reason, ClientRPCs and SyncVars are not updated when this is called immediately.
        //Therefore we wait a second until we spawn it
        StartCoroutine(SpawnNetworkSyncVarsAfterSeconds(1));
    }
    
    private IEnumerator SpawnNetworkSyncVarsAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        SpawnSyncVars();
    }

    private void OnCreateCharacter(NetworkConnection conn, CharacterSpawnMessage message)
    {
        if (_connectedPlayerObjects.ContainsKey(conn)) //already created a player object for this connection!
            return;
        
        GameObject playerObject;
        if (SceneManager.GetActiveScene().name.Contains("Laboratory"))
        {
            playerObject = Instantiate(playerPrefab);
            playerObject.transform.position = message.CharacterPosition;
            playerObject.transform.rotation = message.CharacterRotation;
        }
        else
        {
            playerObject = Instantiate(experimentPlayer);
        }

        string playerName;
        if (_connectedPlayers.ContainsValue(conn))
        {
            playerName = _connectedPlayers.FirstOrDefault(x => x.Value == conn).Key;
        }
        else
        {
            playerName = NetworkNamingService.GetNewPlayerName();
            if (playerName == "")
            {
                playerName = "Scientist " + conn.connectionId;
            }
            _connectedPlayers[playerName] = conn;
        }

        NetworkPlayer networkPlayer = playerObject.GetComponent<NetworkPlayer>();
        networkPlayer.SetName(playerName);

        _connectedPlayerObjects[conn] = networkPlayer;
        NetworkServer.AddPlayerForConnection(conn, playerObject);
    }

    public void RemovePlayerForConnection(NetworkConnection conn)
    {
        _connectedPlayerObjects.Remove(conn);
    }

    private void OnChangeSceneMessage(NetworkConnection conn, ChangeSceneMessage msg)
    {
        if (conn == _connectedPlayers[GetClientInControl()] && CheckSceneValid(msg.SceneName))
        {
            ServerChangeScene(msg.SceneName);
        }
    }

    public bool PortsMapped
    {
        get => _activePortMapping;
        set
        {
            _activePortMapping = value;
            if (!value)
            {
                DisplayMessage("PortMappingFail");
            }
            else if (!_listServer.GetListServerStatus())
            {
                DisplayMessage("ListServerFail");
            }
            else
            {
                DisplayMessage("PortMappingSuccess");
            }
        }
    }

    public string GetClientInControl()
    {
        if (_clientInControl == null)
            _clientInControl = _playerName;
        
        return _clientInControl;
    }

    public List<string> GetAllPlayerNames()
    {
        return _connectedPlayers.Keys.ToList();
    }

    public void ServerGrantControl(string newClientInControl)
    {
        //Tell client currently in control he is not
        NetworkControlMessage msg = new NetworkControlMessage
        {
            InControl = false
        };
        _connectedPlayers[GetClientInControl()].Send(msg);
        
        //Tell new client he is in control
        msg.InControl = true;
        _connectedPlayers[newClientInControl].Send(msg);
        _clientInControl = newClientInControl;
    }

    public void TakeControl()
    {
        if (mode != NetworkManagerMode.Host)
            return;
        ServerGrantControl(_playerName);
    }

    private void SpawnSyncVars()
    {
        GameObject syncVars = Instantiate(preNetworkSyncVars);
        NetworkServer.Spawn(syncVars);
    }

    private void MarkTakenServerNames()
    {
        foreach (var server in _listServer.list.Values)
        {
            NetworkNamingService.ServerNameTaken(server.title);
        }
    }

    #endregion

    #region Client

    private bool _tryClientConnect = true;
    private bool _hasLoadedSceneOnce;
    private bool _isInControl;
    private string _playerName;
    private string _serverName;
    private List<string> _messagesForNextScene = new List<string>();

    public override void OnStartClient()
    {
        base.OnStartClient();
        NetworkClient.RegisterHandler<NetworkControlMessage>(OnNetworkControlMessage);
        if (mode == NetworkManagerMode.ClientOnly)
        {
            _networkDiscovery.StopDiscovery();
            _tryClientConnect = true;
            _hasLoadedSceneOnce = false;
        }
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        if (_tryClientConnect)
        {
            //Connection attempt failed!
            _tryClientConnect = false;
            _messagesForNextScene.Add("ClientConnectFail");
        }
        else if (_hasLoadedSceneOnce)
        {
            //Disconnected from host
            _messagesForNextScene.Add("ClientDisconnect");
        }
        else
        {
            _messagesForNextScene.Add("ServerInExperiment");
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        _playerName = null;
        IsInControl = false;
        _serverName = null;
        _networkDiscovery.StartDiscovery();
        SceneManager.LoadScene(onlineScene);
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        _tryClientConnect = false;
        SendCreatePlayerMessage(conn);
    }

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        _hasLoadedSceneOnce = true;
        base.OnClientSceneChanged(conn);
        SendCreatePlayerMessage(conn);
        if (_isInControl)
        {
            onGetControl.Invoke();
        }
        else
        {
            onLoseControl.Invoke();
        }
    }

    private void SendCreatePlayerMessage(NetworkConnection conn)
    {
        // you can send the message here, or wherever else you want
        CharacterSpawnMessage characterMessage = new CharacterSpawnMessage
        {
            CharacterPosition = _gameManager.GetPlayerPosition(),
            CharacterRotation = _gameManager.GetPlayerRotation()
        };

        conn.Send(characterMessage);
    }
    
    void OnSceneFinishedLoading(Scene scene, LoadSceneMode lsmode)
    {
        foreach (var key in _messagesForNextScene)
        {
            DisplayMessage(key);
        }
        _messagesForNextScene.Clear();
    }

    public bool IsInControl
    {
        get => _isInControl;
        private set
        {
            _isInControl = value;
            if (value)
            {
                onGetControl.Invoke();
            }
            else
            {
                onLoseControl.Invoke();
            }
        }
    }

    public string PlayerName
    {
        get => _playerName;
        set
        {
            _playerName = value;
            DisplayWelcomeMessage(value, _serverName);
        }
    }

    public string ServerName
    {
        get => _serverName;
        set => _serverName = value;
    }

    private void OnNetworkControlMessage(NetworkConnection conn, NetworkControlMessage msg)
    {
        IsInControl = msg.InControl;
    }

    private float _lastEnterSceneTime;
    public void EnterScene(string sceneName)
    {
        if (mode == NetworkManagerMode.Offline)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            if (Time.time - _lastEnterSceneTime < 1) //Otherwise executed multiple times!
                return;
            _lastEnterSceneTime = Time.time;
            
            if (sceneName.Contains("Menu"))
            {
                DisplayMessage("MainMenuDenial");
                return;
            }
            if (_isInControl)
            {
                if (CheckSceneValid(sceneName))
                {
                    ChangeSceneMessage msg = new ChangeSceneMessage
                    {
                        SceneName = sceneName
                    };
                    NetworkClient.connection.Send(msg);
                }
                else
                {
                    DisplayMessage("ExperimentNotEnabled");
                }
            }
            else
            {
                DisplayMessage("ControlDenial");
            }
        }
    }

    #endregion

    public override void OnApplicationQuit()
    {
        if (_activePortMapping)
        {
            _upnp.DeletePortMapping();
        }
        base.OnApplicationQuit();
    }
    
    private void DisplayMessage(string messageKey)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        var message = LanguageManager.Instance.GetString(messageKey);
        _dialogueManager.ShowMessage(message);
    }

    private void DisplayWelcomeMessage(string playerName, string serverName)
    {
        if (_dialogueManager == null)
            _dialogueManager = FindObjectOfType<DialogueManager>();

        if (_dialogueManager == null)
            return;

        string message = LanguageManager.Instance.GetString("WelcomeMessage1") + " " + playerName +
                         "! ";
        message += LanguageManager.Instance.GetString("WelcomeMessage2") + " " + serverName +
                   "!";
        
        _dialogueManager.ShowMessage(message);
    }
}
