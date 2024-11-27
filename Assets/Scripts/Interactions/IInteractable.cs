public interface IInteractable
{
    public void Interact();
    public InteractionType Type() => InteractionType.NOONE;
    public bool IsActive() => true;
}

public enum InteractionType { NOONE, GRAB, LOOKAT, SWITCH }