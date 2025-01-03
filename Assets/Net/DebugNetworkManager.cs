using Mirror;
using UnityEngine;

public class DebugNetworkManager : NetworkManager
{
    public override void Start()
    {
        base.Start();
        StartHost();
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        AllPlayersReady();
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
