using UnityEngine;

public class WindowStartScale : MonoBehaviour
{
    [SerializeField] private AnimationCurve _curve;

    [SerializeField] private GameObject _back;

    private RectTransform _transform;
    private float _curveX;

    private void OnEnable()
    {
        _curveX = 0;
        _transform = GetComponent<RectTransform>();
        _transform.localScale = Vector3.zero;
        
        if (_back != null) { _back.SetActive(true); }
    }

    private void OnDisable()
    {
        if (_back != null) { _back.SetActive(false); }
    }

    private void Update()
    {
        if (_curveX < 1)
            _curveX += Time.unscaledDeltaTime * 3f;
        
        float scale = _curve.Evaluate(_curveX);
        _transform.localScale = Vector3.one * scale;
    }
}
