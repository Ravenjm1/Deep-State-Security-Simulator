using Mirror;
using UnityEngine;

[SelectionBase]
public class TruckDoor : NetworkBehaviour, IInteractable
{
    [SerializeField] private Vector3 _openRotation;
    private float _interactionTime;
    [SyncVar] private bool _isOpened;
    private Quaternion _quaternion;
    private OutlineInteract _outline;

    void Awake()
    {
        _quaternion = transform.localRotation;
        _outline = gameObject.AddComponent<OutlineInteract>();
    }
    
    public void Interact()
    {
        _interactionTime = 1f;
        CmdInteract();
    }

    [Command (requiresAuthority = false)]
    void CmdInteract()
    {
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
    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();
}
