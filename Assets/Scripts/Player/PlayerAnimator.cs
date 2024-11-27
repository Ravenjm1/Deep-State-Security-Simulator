using System;
using Mirror;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private Animator _animator;
    [SyncVar] private float _movement;

    private void Awake() => _animator = GetComponent<Animator>();

    private void Update()
    {
        _animator.SetFloat("Moving", _movement);
    }

    public void MoveAnimation(float value)
    {
        value = Mathf.Clamp(value, 0, 1);
        _movement = Mathf.Lerp(_movement, value, Time.deltaTime * 10f);
    }

    public void JumpAnimation() => _animator.SetTrigger("Jump");
}
