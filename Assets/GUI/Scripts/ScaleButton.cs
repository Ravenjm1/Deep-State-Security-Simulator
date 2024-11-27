using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Button))]
public class ScaleButton : MonoBehaviour
{
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private float _minScale = 0.9f;

    public UnityEvent OnPressed;
    
    private Button _button;
    private RectTransform _transform;
    private bool _isPressed;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _transform = GetComponent<RectTransform>();
    }
    
    private void OnEnable()  => _button.onClick.AddListener(Press);
    private void OnDisable() => _button.onClick.RemoveListener(Press);

    private void Press()
    {
        _isPressed = true;
        //_transform.DOScale(_minScale, 0.3f);
        Invoke(nameof(Unpress), 0.15f);
    }

    private void Unpress()
    {
        OnPressed?.Invoke();
        _isPressed = false;
        //_transform.DOScale(_maxScale, 0.3f);
    }
}
