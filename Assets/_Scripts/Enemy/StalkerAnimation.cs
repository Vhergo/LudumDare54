using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;
    private bool onPause;

    public enum AnimationState
    {
        StalkerWalk,
        StalkerPounce,
        StalkerAttack
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
        if (!onPause) ChangeAnimationState(AnimationState.StalkerWalk);
    }

    public void PlayPounceAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.StalkerPounce);
        Invoke("Unpause", 0.25f);
    }

    public void PlayAttackAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.StalkerAttack);
        Invoke("Unpause", 0.25f);
    }

    void Unpause() {
        onPause = false;
    }
}