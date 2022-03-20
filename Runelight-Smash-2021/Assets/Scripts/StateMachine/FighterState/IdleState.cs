using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runelight.States
{
    public class IdleState : BaseFighterState
    {
        private VelocityComponent velocityComponent;

        public IdleState(StateMachine stateMachine, Animator animator, GameObject fighter) : base(stateMachine, animator, fighter)
        {

        }

        public override void OnStateEnter()
        {
            animator.Play("Idle_Loop");
            velocityComponent = fighter.GetComponent<VelocityComponent>();
        }

        public override void OnStateUpdate()
        {
            if (velocityComponent.velocity.x != 0)
            {
                stateMachine.ChangeState(new WalkingState(stateMachine, animator, fighter));
            }
        }

        public override void OnStateExit()
        {

        }
    }
}
