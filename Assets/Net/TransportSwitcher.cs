using UnityEngine;
using Mirror;
using Mirror.FizzySteam;

public class TransportSwitcher : MonoBehaviour
{
    public TelepathyTransport telepathyTransport;
    public FizzySteamworks fizzyTransport;

    private void Start()
    {
        SetTransport(telepathyTransport);
    }

    public void SetSteam()
    {
        SetTransport(fizzyTransport);
    }

    public void SetLocal()
    {
        SetTransport(telepathyTransport);
    }

    void SetTransport(Transport transport)
    {
        // Устанавливаем новый транспорт в NetworkManager
        NetworkManager.singleton.transport = transport;
        Debug.Log("Transport switched to: " + transport.GetType().Name);
    }
}
