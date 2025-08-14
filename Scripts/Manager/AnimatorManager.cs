using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorManager : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void SetBool(string paramName, bool value)
    {
        if (animator == null)
        {
            return;
        }
        animator.SetBool(paramName, value);
    }

    public void SetFloat(string paramName, float value)
    {
        if(animator != null) 
            animator.SetFloat(paramName, value);
    }

    public void SetTrigger(string paramName)
    {
        if (animator != null)
            animator.SetTrigger(paramName);
    }

    public void SetSpeed(float speed)
    {
        if (animator != null)
            animator.speed = speed;
    }

    public Animator GetAnimator() => animator;
}
