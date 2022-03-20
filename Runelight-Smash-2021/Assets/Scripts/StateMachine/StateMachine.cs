using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runelight.States
{
    public class StateMachine
    {
        public State currentState;

        public void ChangeState(State newState)
        {
            if (currentState != null)
            {
                currentState.OnStateExit();
            }

            currentState = newState;
            currentState.OnStateEnter();
        }

        public void Update()
        {
            if (currentState != null)
            {
                currentState.OnStateUpdate();
            }
        }
    }
}
