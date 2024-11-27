using System;
using Mirror;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private PlayerInteractor _playerInteractor;
    [SerializeField] private GameObject _interactionUI;

    private void Update()
    {
        if (_playerInteractor == null) {
            _playerInteractor = FindFirstObjectByType<PlayerInteractor>();
            return;
        }
        if (_playerInteractor.IsInteractionAvailable() && !_playerInteractor.IsLocked)
            ShowInteraction();
        else
            HideInteraction();
    }

    private void ShowInteraction()
    {
        if (_interactionUI.activeSelf == false)
            _interactionUI.SetActive(true);
    }

    private void HideInteraction()
    {
        if (_interactionUI.activeSelf == true)
            _interactionUI.SetActive(false);
    }
}

