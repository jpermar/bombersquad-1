using System;

namespace bombersquad_ai
{
	namespace fsm
	{
		public class FSM<T>
		{
			private T owner;
			private State<T> currentState;
			private State<T> previousState;

			public FSM (T owner, State<T> currentState, State<T> previousState)
			{
				this.owner = owner;
				this.currentState = currentState;
				this.previousState = previousState;
			}
	
			/*
			public void update ()
			{
				if (this.currentState != null) {
					this.currentState.execute (this.owner);
				}
			}
			*/
	
			public void changeState (State<T> newState)
			{
				this.previousState = this.currentState;
				if (this.currentState != null) {
					this.currentState.exitState (this.owner);
				}
				this.currentState = newState;
				this.currentState.enterState (owner);
			}
	
			public void revertToPreviousState ()
			{
				this.changeState (this.previousState);
			}

			public State<T> getCurrentState ()
			{
				return currentState;
			}

			public State<T> getPreviousState ()
			{
				return previousState;
			}

			public bool isInState (State<T> state)
			{
				return this.currentState.Equals (state);
			}
	
		}
	}
}

