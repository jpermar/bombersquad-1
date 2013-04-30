using System;

namespace bombersquad_ai
{
	namespace fsm
	{
		public interface State<T>
		{
		
			void enterState (T owner);

			Action getAction (T owner);

			void exitState (T owner);
			
			bool fail(T owner);
			
			bool complete(T owner);
		}
	}
}

