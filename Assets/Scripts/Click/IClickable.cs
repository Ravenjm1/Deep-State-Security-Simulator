using UnityEngine;

public interface IClickable
{
    public void Click();
    public void Pressed();
    public void Released();
    public bool IsActive();
    public bool IsPressed();
    public void Hover();
    public void Unhover();
}