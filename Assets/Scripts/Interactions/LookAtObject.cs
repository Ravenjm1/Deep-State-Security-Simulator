using System;
using Unity.Cinemachine;
using UnityEngine;

public class LookAtObject : MonoBehaviour, IInteractable
{
    [SerializeField] CinemachineCamera lookCamera;
    public bool IsLookedAt {get; private set; }
    public event Action OnStartLook = delegate {  };
    public event Action OnStopLook = delegate {  };
    private OutlineInteract _outline;

    void Awake() => _outline = gameObject.AddComponent<OutlineInteract>();
    public void Interact()
    {
        if (LocationContext.GetDependency.Player.TryToLookAt(this))
        {
            IsLookedAt = true;
            lookCamera.Priority = 20;
            OnStartLook();
        }
    }
    public void StopLooking()
    {
        IsLookedAt = false;
        lookCamera.Priority = 0;
        OnStopLook();
    }

    public bool IsActive() => !IsLookedAt;

    public InteractionType Type() => InteractionType.LOOKAT;
    public string GetInteractText() => "Look";
    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();
}
