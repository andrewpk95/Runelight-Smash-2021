using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runelight.States
{
    public class FighterStateMachine : StateMachine
    {
        public FighterStateMachine(Animator animator, GameObject fighter) : base()
        {
            this.ChangeState(new IdleState(this, animator, fighter));
        }
    }
}
