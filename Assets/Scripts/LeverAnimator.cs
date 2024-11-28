using UnityEngine;
using UnityEngine.Events;

public class LeverAnimator : MonoBehaviour
{
    private Animator animator;

    public UnityEvent onAnimationEndCalled;
    private bool isAnimationPlaying = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        SetToStartFrame(); // Устанавливаем анимацию на начальный кадр
    }

    private void SetToStartFrame()
    {
        // Устанавливаем анимацию на начальный кадр (Idle)
        animator.Play("Idle", 0, 0f);
        animator.speed = 0; // Останавливаем анимацию
    }

    public void PlayLeverAnimation()
    {
        if (!isAnimationPlaying)
        {
            isAnimationPlaying = true;
            animator.speed = 1; // Включаем анимацию
            animator.SetTrigger("PlayAnimation"); // Запускаем анимацию
            StartCoroutine(ResetToIdleAfterAnimation());
        }
    }

    private System.Collections.IEnumerator ResetToIdleAfterAnimation()
    {
        // Ждём, пока анимации завершатся
        yield return new WaitForSeconds(5);//GetAnimationLength("Po_Bo|Level_Down") + GetAnimationLength("Po_Bo|Level_Down"));

        // Сбрасываем в исходное состояние
        isAnimationPlaying = false;
        onAnimationEndCalled.Invoke();
    }

    private float GetAnimationLength(string animationName)
    {
        // Получаем длину анимации
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            Debug.Log("clip.name: " + clip.name);
            if (clip.name == animationName)
            {
                return clip.length;
            }
        }
        Debug.LogWarning($"Animation {animationName} not found!");
        return 0f;
    }
}
