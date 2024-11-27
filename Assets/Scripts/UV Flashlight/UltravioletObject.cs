using System;
using UnityEngine;

public class UltravioletObject : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    private Material _sharedMaterial;

    private Light _uvLight;

    private void Awake()
    {
        _sharedMaterial = _renderer.sharedMaterial;
        _uvLight = FindAnyObjectByType<UVFlashlight>().Source;
    }

    private void Update()
    {
        _sharedMaterial.SetVector("_LightPosition", _uvLight.transform.position);
        _sharedMaterial.SetVector("_LightDirection", -_uvLight.transform.forward);
        _sharedMaterial.SetFloat("_LightRange", _uvLight.range);
        _sharedMaterial.SetFloat("_LightAngle", _uvLight.spotAngle/3);
    }
}
