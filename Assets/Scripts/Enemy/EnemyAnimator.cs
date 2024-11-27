using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _animator;
    private float _movement;
    private bool _attacking;
    private bool _die;

    private void Awake() => _animator = GetComponent<Animator>();

    private void Update()
    {
        _animator.SetFloat("Moving", _movement);
        _animator.SetBool("Attacking", _attacking);
        _animator.SetBool("Die", _die);
    }

    public void MoveAnimation(float value)
    {
        value = Mathf.Clamp(value, 0, 1);
        _movement = Mathf.Lerp(_movement, value, Time.deltaTime * 10f);
    }

    public void AttackAnimation(bool value) 
    {
        _attacking = value;
    }

    public void DieAnimation()
    {
        _die = true;
    }
}
