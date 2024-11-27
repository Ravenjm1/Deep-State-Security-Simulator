using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera followCamera;
    CinemachineCamera playerCamera;
    GameObject head;
    PlayerStats playerStats;
    void OnEnable() => LocationContext.OnReady += Init;
    void OnDisable() => LocationContext.OnReady -= Init;

    void Awake()
    {
        playerCamera = GetComponent<CinemachineCamera>();

    }

    void Init()
    {
        playerStats = LocationContext.GetDependency.Player.GetComponent<PlayerStats>();
        playerStats.OnDead += SwitchToFollow;
        playerStats.OnResurect += SwitchToPov;
    }

    public void AssignCameraFollow(PlayerController playerController)
    {
        if (playerCamera != null)
        {
            head = playerController.GetComponentInChildren<PlayerInteractor>().gameObject;
            // Устанавливаем параметр follow на transform этого игрока
            playerCamera.Follow = head.transform;
            followCamera.Follow = head.transform;
        }
        else
        {
            Debug.LogWarning("Virtual Camera not found in the scene.");
        }
    }

    void SwitchToFollow()
    {
        followCamera.Priority = 20;
        foreach (var player in LocationContext.GetDependency.ListPlayers)
        {
            if (!player.GetComponent<PlayerStats>().IsDead)
            {
                followCamera.Follow = player.transform;
                return;
            }
        }
    }
    void SwitchToPov()
    {
        followCamera.Priority = 0;
        followCamera.Follow = head.transform;
    }
}
