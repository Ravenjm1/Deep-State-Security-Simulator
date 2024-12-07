using UnityEngine;

[SelectionBase]
public class TruckDoor : MonoBehaviour, IInteractable
{
    [SerializeField] private Vector3 _openRotation;
    private float _interactionTime;
    private bool _isOpened;
    private Quaternion _quaternion;

    void Awake()
    {
        _quaternion = transform.localRotation;
    }
    
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
                Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_quaternion.eulerAngles +_openRotation), Time.deltaTime * 8f);
        }
        else
        {
            transform.localRotation =
                Quaternion.Slerp(transform.localRotation, _quaternion, Time.deltaTime * 8f);
        }
    }

    public bool IsActive() => _interactionTime <= 0;
    public string GetInteractText() => "Open";
}
