using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ButtonClicker : MonoBehaviour, IClickable
{
    private Vector3 originalScale;
    private Renderer buttonRenderer;
    public Color highlightColor = Color.yellow;
    public Color originalColor;
    // UnityEvent позволяет вам закидывать методы через инспектор
    public UnityEvent onClickCalled;
    Vector3 pressedScale = new Vector3(0.9f, 0.9f, 0.9f);
    void Start()
    {
        originalScale = transform.localScale;
        buttonRenderer = GetComponent<Renderer>();
        originalColor = buttonRenderer.material.color;
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

    public void Hover()
    {
        // Выделение кнопки при наведении
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = highlightColor;
        }
    }

    public void Unhover()
    {
        // Снятие выделения кнопки при уходе курсора
        if (buttonRenderer != null)
        {
            buttonRenderer.material.color = originalColor;
        }
    }

    public bool IsActive()
    {
        return true;
    }

    public void Click()
    {
        
    }
}