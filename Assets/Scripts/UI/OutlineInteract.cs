using UnityEngine;

public class OutlineInteract : MonoBehaviour
{
    private Outline outline;
    void Awake()
    {
        outline = gameObject.AddComponent<Outline>();
        outline.OutlineColor = Color.yellow;
        outline.OutlineWidth = 10;
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
}
