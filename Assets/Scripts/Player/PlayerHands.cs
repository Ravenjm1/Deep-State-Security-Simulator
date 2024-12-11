using System;
using System.Collections;
using Mirror;
using UnityEngine;

public class PlayerHands : NetworkBehaviour
{
    [field: SerializeField] public Transform Center { private set; get; }
    private Camera _mainCamera;
    private Transform _followTarget;
    
    private float _watchOffset = 1f;
    
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        _mainCamera = Camera.main;

        _followTarget = _mainCamera.transform;
        transform.parent = null;
    }

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            _followTarget.rotation * Quaternion.Euler(0f, 30 /** _watchOffset*/, 0f), Time.deltaTime * 20f);

        SetWatching();
    }

    private void LateUpdate()
    {
        if (!isLocalPlayer)
            return;
        transform.position = _followTarget.position - (Vector3.up * (0.4f * _watchOffset));
    }

    private void SetWatching()
    {
        if (InputManager.Instance.WatchHolding())
            _watchOffset = Mathf.Lerp(_watchOffset, 0f, Time.deltaTime * 10f);
        else
            _watchOffset = Mathf.Lerp(_watchOffset, 1f, Time.deltaTime * 20f);
    }
}
