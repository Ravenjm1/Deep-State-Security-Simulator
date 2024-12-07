using System;
using Mirror;
using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    private PlayerInteractor _playerInteractor;
    [SerializeField] private GameObject _interactionUI;
    private TMP_Text _interactionText;

    void Awake() => _interactionText = _interactionUI.GetComponentInChildren<TMP_Text>();
    private void Update()
    {
        if (_playerInteractor == null) {
            var player = LocationContext.GetDependency.Player;
            if (player != null) {
                _playerInteractor = LocationContext.GetDependency.Player.GetComponentInChildren<PlayerInteractor>();
            }
            return;
        }
        if (_playerInteractor.IsInteractionAvailable() && !_playerInteractor.IsLocked)
        {
            ShowInteraction();
        }
        else
            HideInteraction();
    }

    private void ShowInteraction()
    {
        _interactionText.text = "E - " + _playerInteractor.GetInteractionObject().GetInteractText();
        if (_interactionUI.activeSelf == false) 
            _interactionUI.SetActive(true);
    }

    private void HideInteraction()
    {
        if (_interactionUI.activeSelf == true)
            _interactionUI.SetActive(false);
    }
}

