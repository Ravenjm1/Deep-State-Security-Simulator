using System;
using UnityEngine;

public class UVFlashlight : MonoBehaviour
{
    [field: SerializeField] public Light Source { private set; get; }
    [SerializeField] private GrabObject _connectedGrab;

    private void Update()
    {
        if (!_connectedGrab.IsGrabbed)
            return;
        
        Vector3 world = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f));
        transform.LookAt(world);
    }
}
