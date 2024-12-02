using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;

public class PanelDecide : NetworkBehaviour
{
    [SerializeField] List<Transform> explosionTransform;
    [SerializeField] GameObject _explosionPrefab;
    CarSpawner carSpawner; // Ссылка на CarSpawner
    private Object[] walls;
    void Awake()
    {
        carSpawner = GetComponent<CarSpawner>();

        walls = FindObjectsByType(typeof(ModularWall), FindObjectsSortMode.None);
    }

    // Вызывается при нажатии кнопки "Пропустить"
    public void OnPassButtonPressed()
    {
        Debug.Log("OnPassButtonPressed");
        CmdRequestPassCar();
    }

    // Вызывается при нажатии кнопки "Отклонить"
    public void OnRejectButtonPressed()
    {
        Debug.Log("OnRejectButtonPressed");
        CmdRequestCancelCar();
    }

    [Command (requiresAuthority = false)]
    void CmdRequestPassCar()
    {
        if (carSpawner != null)
        {
            carSpawner.CurrentCarPass();
        }
    }

    // Команда на сервер для обработки "Отказа"
    [Command (requiresAuthority = false)]
    void CmdRequestCancelCar()
    {
        if (carSpawner != null)
        {
            RpcCloseDoors();
            StartCoroutine(ExplosionCar());
        }
    }

    [Server]
    IEnumerator ExplosionCar()
    {
        yield return new WaitForSeconds(1f);

        RpcExplosion();
        carSpawner.CurrentCarCancel();
        StartCoroutine(TimerOpenDoors());
    }

    [Server]
    IEnumerator TimerOpenDoors()
    {
        yield return new WaitForSeconds(2f);

        RpcOpenDoors();
    }

    [ClientRpc]
    void RpcExplosion()
    {
        foreach (var expl in explosionTransform)
        {
            Instantiate(_explosionPrefab, expl.position, Quaternion.identity);
        }
    }

    [ClientRpc]
    void RpcCloseDoors()
    {
        foreach (var wall in walls)
        {
            wall.GetOrAddComponent<ModularWall>().Close();
        }
    }

    [ClientRpc]
    void RpcOpenDoors()
    {
        foreach (var wall in walls)
        {
            wall.GetOrAddComponent<ModularWall>().Open();
        }
    }
}

