using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationController
{
    private Animator animator;

    // Animation States
    private const string IdleAnimation = "Idle";
    private const string AttackAnimation = "Attack";
    private const string RunAnimation = "Run";
    private const string JumpAnimation = "Jump";
    private const string DeathAnimation = "Death";

    // Constructor
    public AnimationController(Animator animator)
    {
        this.animator = animator;
    }

    // Play a specific animation
    public void PlayIdle()
    {
        ResetAllAnimations();
        animator.SetBool(IdleAnimation, true);
    }

    public void PlayAttack()
    {
        ResetAllAnimations();
        animator.SetBool(AttackAnimation, true);
    }

    public void PlayRun()
    {
        ResetAllAnimations();
        animator.SetBool(RunAnimation, true);
    }

    public void PlayJump()
    {
        ResetAllAnimations();
        animator.SetBool(JumpAnimation, true);
    }

    public void PlayDeath()
    {
        ResetAllAnimations();
        animator.SetBool(DeathAnimation, true);
    }

    // Stop all animations
    public void ResetAllAnimations()
    {
        animator.SetBool(IdleAnimation, false);
        animator.SetBool(AttackAnimation, false);
        animator.SetBool(RunAnimation, false);
        animator.SetBool(JumpAnimation, false);
        animator.SetBool(DeathAnimation, false);
    }
}
