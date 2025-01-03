using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class MyNetworkManager : NetworkManager
{
    public string lobbyScene = "LobbyScene";
    public string gameScene = "GameScene";

    private bool gameStarted = false; // Флаг, который отслеживает начало игры
    private List<NetworkConnectionToClient> readyPlayers = new List<NetworkConnectionToClient>();
    private int readyPlayerCount = 0; // Счетчик готовых игроков

    public override void OnStartHost()
    {
        base.OnStartHost();
        //ServerChangeScene(lobbyScene); // Перемещаем хоста в лобби
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);

        if (sceneName == gameScene)
        {
            //gameStarted = true; // Игра началась
            /*
            foreach (var conn in NetworkServer.connections)
            {
                if (conn.Value.identity == null) // Если игрок не был заспавнен
                {
                    OnServerAddPlayer(conn.Value);
                }
            }
            */
            readyPlayerCount = 0;
            readyPlayers.Clear(); // Очищаем список игроков при смене сцены
        }
        
    }
    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        Debug.Log("Client connected: " + conn.connectionId);
        // Если игра началась, отключаем новых игроков
        if (gameStarted)
        {
            conn.Disconnect();
        }
        else
        {
            base.OnServerConnect(conn);
        }
    }
    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);
        Debug.Log("Player added for connection: " + conn.connectionId);
    }
    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        // Увеличиваем счетчик готовых игроков
        readyPlayerCount++;

        // Проверяем, все ли игроки готовы
        if (readyPlayerCount == NetworkServer.connections.Count)
        {
            Debug.Log("Все игроки готовы. Можно начинать игру.");
            AllPlayersReady();
        }
    }

    void AllPlayersReady()
    {
        // Здесь активируем объект LocationContext или другое действие
        LocationContext locationContext = FindFirstObjectByType<LocationContext>();
        if (locationContext != null)
        {
            locationContext.PlayerAreReady = true;;
        }
    }
}
