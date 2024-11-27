using UnityEngine;

[SelectionBase]
public class TruckDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 _openRotation;
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
                Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_openRotation), Time.deltaTime * 8f);
        }
        else
        {
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 8f);
        }
    }

    public bool IsActive() => _interactionTime <= 0;
}
