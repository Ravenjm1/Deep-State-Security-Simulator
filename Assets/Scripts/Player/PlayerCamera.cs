using UnityEngine;
using Mirror;
using Unity.Cinemachine;

public class PlayerCamera : NetworkBehaviour
{
    public GameObject head;
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        AssignCameraFollow();
    }

    private void AssignCameraFollow()
    {
        // Ищем Virtual Camera на сцене
        CinemachineCamera camera = FindFirstObjectByType<CinemachineCamera>();

        if (camera != null)
        {
            // Устанавливаем параметр follow на transform этого игрока
            camera.Follow = head.transform;
            Debug.Log("Camera follow set to: " + head.gameObject.name);
        }
        else
        {
            Debug.LogWarning("Virtual Camera not found in the scene.");
        }
    }
}
