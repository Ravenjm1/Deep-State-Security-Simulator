using UnityEngine;
using UnityEngine.UI;
using Steamworks;

public class LobbyUI : MonoBehaviour
{
    public SteamLobby steamLobby;

    // UI элементы
    public Button createLobbyButton;
    public Button startGameButton;
    public Button inviteFriendButton;

    private void Start()
    {
        createLobbyButton.onClick.AddListener(steamLobby.CreateLobby);

        // Изначально кнопка "Начать игру" неактивна
        //startGameButton.gameObject.SetActive(false);
    }

    // Метод для обновления состояния кнопки "Начать игру"
    public void UpdateStartGameButton(bool isOwner)
    {
        startGameButton.gameObject.SetActive(isOwner);
    }
}
