namespace Runelight.States
{
    public abstract class State
    {
        public string name;

        protected StateMachine stateMachine;

        public State(StateMachine stateMachine)
        {
            this.name = this.GetType().Name;
            this.stateMachine = stateMachine;
        }

        public abstract void OnStateEnter();

        public abstract void OnStateUpdate();

        public abstract void OnStateExit();
    }
}
