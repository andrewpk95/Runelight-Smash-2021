using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runelight.States
{
    public abstract class BaseFighterState : State
    {
        protected Animator animator;
        protected GameObject fighter;

        public BaseFighterState(StateMachine stateMachine, Animator animator, GameObject fighter) : base(stateMachine)
        {
            this.animator = animator;
            this.fighter = fighter;
        }
    }
}
