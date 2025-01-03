using UnityEngine;
using Steamworks;
using Mirror;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class SteamLobby : MonoBehaviour
{
    // Callbacks
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> joinRequest;
    protected Callback<LobbyEnter_t> lobbyEntered;

    // Lobby Data
    private CSteamID currentLobbyID;
    private const string HostAddressKey = "HostAddress";

    // Network Manager
    private MyNetworkManager networkManager;

    void OnEnable() => SteamManager.OnInitialized += Init;
    void OnDisable() => SteamManager.OnInitialized -= Init;

    private void Init()
    {
        networkManager = (MyNetworkManager)NetworkManager.singleton;

        // Initialize Callbacks
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);

        SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());
        SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log("Steam Lobby Manager Initialized");

        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Create a new lobby
    /// </summary>
    public void CreateLobby()
    {
        // Оставляем старое лобби, если оно существует
        SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());

        // Создаём новое лобби
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, networkManager.maxConnections);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.LogError("Failed to create lobby.");
            return;
        }

        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // Set the host address to the lobby data
        SteamMatchmaking.SetLobbyData(currentLobbyID, HostAddressKey, SteamUser.GetSteamID().ToString());

        // Start hosting the game
        networkManager.StartHost();

        Debug.Log("Lobby created successfully.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            // Оставляем старое лобби, если оно существует
            SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());
        }
    }

    private void OnJoinRequest(GameLobbyJoinRequested_t callback)
    {
        // Join the lobby when invited
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        currentLobbyID = new CSteamID(callback.m_ulSteamIDLobby);

        // If we are not the host, start the client
        if (!NetworkServer.active)
        {
            string hostAddress = SteamMatchmaking.GetLobbyData(currentLobbyID, HostAddressKey);
            networkManager.networkAddress = hostAddress;
            networkManager.StartClient();
            Debug.Log("Joined lobby. Starting client.");
        }
        else
        {
            Debug.Log("Hosting lobby.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (SceneManager.GetActiveScene().name == "LobbyScene")
            {
                InviteFriends();
            }
        }
    }

    /// <summary>
    /// Invite friends through Steam Overlay
    /// </summary>
    public void InviteFriends()
    {
        SteamFriends.ActivateGameOverlayInviteDialog(currentLobbyID);
    }

    /// <summary>
    /// Check if the current player is the host
    /// </summary>
    public bool IsLobbyHost()
    {
        return SteamMatchmaking.GetLobbyOwner(currentLobbyID) == SteamUser.GetSteamID();
    }

    /// <summary>
    /// Start the game (for the host)
    /// </summary>
    public void StartGame()
    {
        if (!IsLobbyHost())
        {
            Debug.LogError("Only the host can start the game.");
            return;
        }

        // Перевод всех участников на GameScene
        Debug.Log("Starting the game...");

        // Загрузка игровой сцены (будет синхронизирована с клиентами)
        networkManager.ServerChangeScene("LobyyScene");
    }

    private void OnApplicationQuit()
    {
        // Оставляем лобби перед выходом из игры
        SteamMatchmaking.LeaveLobby(SteamUser.GetSteamID());
    }
}
