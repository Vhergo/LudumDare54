using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GruntAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;
    private bool onPause;

    public enum AnimationState
    {
        GruntWalk,
        GruntAttack
    }

    void Start() {
        anim = GetComponent<Animator>();
        onPause = false;
    }

    public void ChangeAnimationState(AnimationState newState) {
        // stop the same animation from interrupting itself
        if (currentState == newState) return;

        anim.Play(newState.ToString()); // play animation
        currentState = newState; // upadate the current state
    }

    public void PlayWalkAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.GruntWalk);
    }

    public void PlayAttackAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.GruntAttack);
        Invoke("Unpause", 0.25f);
    }

    void Unpause() {
        onPause = false;
    }
}