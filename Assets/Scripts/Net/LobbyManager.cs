using UnityEngine;
using Mirror;
using Steamworks;
public class LobbyManager : NetworkBehaviour
{
    MyNetworkManager networkManager;

    void Start()
    {
        networkManager = (MyNetworkManager)NetworkManager.singleton;
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
            //LockLobby();
            
            networkManager.ServerChangeScene(networkManager.gameScene);
        }
    }

    private void LockLobby()
    {
        // Получаем текущий Steam лобби ID
        CSteamID lobbyID = SteamMatchmaking.GetLobbyOwner(SteamUser.GetSteamID());
        
        // Закрываем лобби, делая его невидимым для других игроков
        SteamMatchmaking.SetLobbyType(lobbyID, ELobbyType.k_ELobbyTypeInvisible);
        
        // Дополнительно, если хотите, можете закрыть возможность приглашений
        SteamMatchmaking.SetLobbyJoinable(lobbyID, false);
    }
}
