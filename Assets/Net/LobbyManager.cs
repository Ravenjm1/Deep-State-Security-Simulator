using UnityEngine;
using Mirror;
using Steamworks;
using TMPro;
using Unity.VisualScripting;
public class LobbyManager : NetworkBehaviour
{
    MyNetworkManager networkManager;
    [SerializeField] TMP_Text startText;
    [SerializeField] TMP_Text inviteText;

    void Start()
    {
        networkManager = (MyNetworkManager)NetworkManager.singleton;
        if (isServer)
        {
            startText.gameObject.SetActive(true);
        }
        if (SteamManager.Initialized)
        {
            inviteText.gameObject.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartGame();
        }
    }

    // Проверяем, является ли текущий игрок хостом
    public void StartGame()
    {
        if (NetworkServer.active && NetworkManager.singleton.isNetworkActive)
        {
            LockLobby();
            
            networkManager.ServerChangeScene(networkManager.gameScene);
        }
    }

    private void LockLobby()
    {
        if (SteamManager.Initialized)
        {
            // Получаем текущий Steam лобби ID
            CSteamID lobbyID = SteamMatchmaking.GetLobbyOwner(SteamUser.GetSteamID());
            
            // Закрываем лобби, делая его невидимым для других игроков
            SteamMatchmaking.SetLobbyType(lobbyID, ELobbyType.k_ELobbyTypeInvisible);
            
            // Дополнительно, если хотите, можете закрыть возможность приглашений
            SteamMatchmaking.SetLobbyJoinable(lobbyID, false);
        }
    }
}
