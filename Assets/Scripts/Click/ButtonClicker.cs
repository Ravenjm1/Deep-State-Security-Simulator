using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonClicker : MonoBehaviour, IClickable
{
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Renderer buttonRenderer;
    private bool isPresed = false;
    public UnityEvent onClickCalled;
    Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    private OutlineInteract _outline;
    void Start()
    {
        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
        buttonRenderer = GetComponent<Renderer>();
        _outline = gameObject.AddComponent<OutlineInteract>();
        _outline.SetVisibleMode();
    }

    public void Pressed()
    {
        isPresed = true;
        // Сжатие кнопки при нажатии
        transform.localPosition = originalPosition - new Vector3(0, 0.05f, 0);
        //transform.localScale = Vector3.Scale(pressedScale, originalScale);
    }
    public void Released()
    {
        isPresed = false;
        onClickCalled.Invoke();
        transform.localPosition = originalPosition;
        // Восстановление оригинального масштаба при отпускании кнопки
        //transform.localScale = originalScale;
    }

    public void Hover() => _outline.EnableOutline();
    public void Unhover() => _outline.DisableOutline();

    public bool IsActive() => true;

    public bool IsPressed() => isPresed;

    public void Click()
    {
        
    }
}