using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    private AnimationState currentState;
    private bool onPause;

    public enum AnimationState
    {
        PlayerIdle,
        PlayerWalk,
        PlayerDash
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

    public void PlayIdleAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.PlayerIdle);
    }

    public void PlayWalkAnim() {
        if (!onPause) ChangeAnimationState(AnimationState.PlayerWalk);
    }

    public void PlayDashAnim() {
        onPause = true;
        ChangeAnimationState(AnimationState.PlayerDash);
        Invoke("Unpause", 0.25f);
    }

    void Unpause() {
        onPause = false;
    }
}