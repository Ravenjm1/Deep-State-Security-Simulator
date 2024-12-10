using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonClicker : MonoBehaviour, IClickable
{
    private Vector3 originalScale;
    private Renderer buttonRenderer;
    public Color highlightColor = Color.yellow;
    public Color originalColor;
    public UnityEvent onClickCalled;
    Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    private OutlineInteract _outline;
    void Start()
    {
        originalScale = transform.localScale;
        buttonRenderer = GetComponent<Renderer>();
        originalColor = buttonRenderer.material.color;
        _outline = gameObject.AddComponent<OutlineInteract>();
        _outline.SetVisibleMode();
    }

    public void Pressed()
    {
        // Сжатие кнопки при нажатии
        transform.localScale = Vector3.Scale(pressedScale, originalScale);
    }
    public void Released()
    {
        onClickCalled.Invoke();
        // Восстановление оригинального масштаба при отпускании кнопки
        transform.localScale = originalScale;
    }

    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();

    public bool IsActive()
    {
        return true;
    }

    public void Click()
    {
        
    }
}