using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TruckMoveAnimation : MonoBehaviour
{
    [SerializeField] private Car _carController;
    [SerializeField] private Transform _base;

    [SerializeField] private Transform[] _wheels;

    private float _engineAnimationTime;
    private float _engineOffset;

    private float _wheelRotationSpeed;

    private void Start()
    {
        _wheelRotationSpeed = 150;
    }

    private void Update()
    {
        Vector3 engineOffsetPosition = new Vector3(0, 0, _engineOffset * 0.005f);
        _base.transform.localPosition = Vector3.Lerp(_base.transform.localPosition, engineOffsetPosition, Time.deltaTime * 15f);
        
        _engineAnimationTime += Time.deltaTime;
        if (_engineAnimationTime >= 0.03f)
        {
            _engineAnimationTime = 0f;
            _engineOffset = _carController.IsMoving ? Random.Range(0f, 5f) : Random.Range(0f, 1f);
        }

        if (!_carController.IsMoving)
            _wheelRotationSpeed = Mathf.Lerp(_wheelRotationSpeed, 0, Time.deltaTime * 15f);
        
        foreach (var wheel in _wheels)
        {
            wheel.Rotate(Vector3.up, Time.deltaTime * _wheelRotationSpeed);
        }
    }
}
