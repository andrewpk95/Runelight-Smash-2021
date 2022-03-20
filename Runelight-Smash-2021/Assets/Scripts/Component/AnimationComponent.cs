using System.Collections;
using System.Collections.Generic;
using Runelight.States;
using UnityEngine;

public class AnimationComponent : MonoBehaviour
{
    // Required Components
    public Animator animator;
    private StateMachine stateMachine;

    // Debug purposes
    [SerializeField]
    private string currentState;

    void Start()
    {
        if (!animator)
        {
            this.enabled = false;
        }
        stateMachine = new FighterStateMachine(animator, gameObject);
    }

    void FixedUpdate()
    {
        stateMachine.Update();

        if (stateMachine.currentState != null)
        {
            currentState = stateMachine.currentState.name;
        }
    }
}
