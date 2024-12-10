public interface IInteractable
{
    public void Interact();
    public InteractionType Type() => InteractionType.NOONE;
    public bool IsActive() => true;
    public string GetInteractText() => "Interact";
    public void Hover();
    public void Unhover();
}

public enum InteractionType { NOONE, GRAB, LOOKAT, SWITCH }