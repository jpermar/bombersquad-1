using System;

namespace bombersquad_ai
{
	public interface Strategy
	{
		Action[] GetMoves();
		bool IsStratExecuted();
		bool CannotExecute();
	}
}

