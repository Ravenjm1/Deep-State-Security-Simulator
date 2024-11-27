using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kcp2k;
using Mirror;
using UnityEngine;

public class LocationContext : NetworkBehaviour
{
    public static LocationContext GetDependency;

    public PlayerController Player { get; private set; }
    public List<PlayerController> ListPlayers { get; private set; }
    public GrabHolder GrabHolder { get; private set;  }
    public bool PlayerAreReady { get; set;  }
    public LocationManager locationManager{ get; private set; }

    public static event Action OnReady = delegate {  };
    public bool Ready = false;

    private void OnEnable()  => GetDependency = this;
    private void OnDisable() => GetDependency = null;
    
    [Server]
    private IEnumerator Start()
    {
        while (!PlayerAreReady)
        {
            yield return null;
        }
        RpcReady();
    }

    [ClientRpc]
    void RpcReady()
    {
        // GetPlayer;
        Player = NetworkClient.localPlayer.GetComponent<PlayerController>();//FindFirstObjectByType<PlayerController>();
        GrabHolder = FindFirstObjectByType<GrabHolder>();
        locationManager = GetComponent<LocationManager>();

        ListPlayers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None).ToList();
        
        // Убираем столкновение между игроками
        IngonePlayersCollision();
        // Signal that context is ready;
        Debug.Log("Можно игратц");
        OnReady();
        Ready = true;
    }

    private void IngonePlayersCollision()
    {
        // Перебираем пары объектов, чтобы отключить коллизии между ними
        for (int i = 0; i < ListPlayers.Count; i++)
        {
            for (int j = i + 1; j < ListPlayers.Count; j++)
            {
                var player1 = ListPlayers[i].GetComponent<CharacterController>();
                var player2 = ListPlayers[j].GetComponent<CharacterController>();
                Physics.IgnoreCollision(player1, player2);
            }
        }
    }
}