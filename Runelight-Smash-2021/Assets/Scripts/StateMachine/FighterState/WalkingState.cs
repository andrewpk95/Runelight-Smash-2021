using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runelight.States
{
    public class WalkingState : BaseFighterState
    {
        private VelocityComponent velocityComponent;

        public WalkingState(StateMachine stateMachine, Animator animator, GameObject fighter) : base(stateMachine, animator, fighter)
        {

        }

        public override void OnStateEnter()
        {
            animator.Play("Walk_Fast");
            velocityComponent = fighter.GetComponent<VelocityComponent>();
        }

        public override void OnStateUpdate()
        {
            if (velocityComponent.velocity.x == 0)
            {
                stateMachine.ChangeState(new IdleState(stateMachine, animator, fighter));
            }
        }

        public override void OnStateExit()
        {

        }
    }
}
