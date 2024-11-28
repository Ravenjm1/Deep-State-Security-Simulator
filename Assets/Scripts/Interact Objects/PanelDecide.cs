using UnityEngine;
using Mirror;
using Mirror.Examples.Basic;
using Unity.VisualScripting;

public class PanelDecide : NetworkBehaviour
{
    CarSpawner carSpawner; // Ссылка на CarSpawner

    void Awake()
    {
        carSpawner = GetComponent<CarSpawner>();
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
        Debug.Log("CmdRequestPassCar");
        if (carSpawner != null)
        {
            carSpawner.CurrentCarPass();
        }
    }

    // Команда на сервер для обработки "Отказа"
    [Command (requiresAuthority = false)]
    void CmdRequestCancelCar()
    {
        Debug.Log("CmdRequestCancelCar");
        if (carSpawner != null)
        {
            carSpawner.CurrentCarCancel();
        }
    }
}

