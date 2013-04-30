using System;

namespace bombersquad_ai
{
	public class BlockEscape : Strategy
	{
		public BlockEscape ()
		{
		}
		
		public Action[] GetMoves() {
			return null;
		}
		
		public bool IsStratExecuted() {
			return false;
		}
		
		public bool CannotExecute() {
			return false;
		}
	}
}

