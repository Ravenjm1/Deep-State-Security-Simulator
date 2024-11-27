using UnityEngine;

public class PulseScale : MonoBehaviour
{
    [SerializeField] private AnimationCurve _pulseCurve;

    private float _curveX;
    private bool _isPulsed;

    [SerializeField] private bool _alwaysPulse;
    [SerializeField] private float _timeScale = 1f;
    
    private void Update()
    {
        if (_alwaysPulse == true)
            _isPulsed = true;
        
        if (_isPulsed)
        {
            _curveX += Time.unscaledDeltaTime * 3f * _timeScale;
            transform.localScale = Vector3.one * (1 + (_pulseCurve.Evaluate(_curveX) * 0.3f));

            if (_curveX >= 1)
            {
                _curveX = 0;
                _isPulsed = false;
            }
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    public float GetCurveX() => _pulseCurve.Evaluate(_curveX);

    public void DoPulse()
    {
        _isPulsed = true;
        //_curveX = 0;
    }

    public void StopPulse() => _isPulsed = false;
}
