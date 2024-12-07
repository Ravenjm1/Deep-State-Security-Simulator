using System;
using UnityEngine;

[SelectionBase]
public class TruckBack : MonoBehaviour, IInteractable
{
    private float _interactionTime;
    private bool _isOpened;
    
    public void Interact()
    {
        _interactionTime = 1f;
        
        _isOpened = !_isOpened;
    }

    private void Update()
    {
        if (_interactionTime > 0)
            _interactionTime -= Time.deltaTime;
        
        if (_isOpened)
        {
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, -125, 0), Time.deltaTime * 8f);
        }
        else
        {
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 8f);
        }
    }

    public bool IsActive() => _interactionTime <= 0;
    public string GetInteractText() => "Open";
}
