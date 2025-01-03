using UnityEngine;
using UnityEngine.Events;

public class Switcher : MonoBehaviour, IInteractable 
{
    public UnityEvent onClickCalled;
    bool switched = false;
    private OutlineInteract _outline;

    void Awake() => _outline = gameObject.AddComponent<OutlineInteract>();

    public void Interact()
    {
        onClickCalled.Invoke();
        switched = true;
    }

    public void SwitchOff()
    {
        switched = false;
    }

    public bool IsActive() => !switched;

    public InteractionType Type() => InteractionType.SWITCH;

    public string GetInteractText() => "Switch on";
    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();
}
