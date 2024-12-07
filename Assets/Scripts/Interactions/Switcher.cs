using UnityEngine;
using UnityEngine.Events;

public class Switcher : MonoBehaviour, IInteractable 
{
    public UnityEvent onClickCalled;

    bool switched = false;

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
}
