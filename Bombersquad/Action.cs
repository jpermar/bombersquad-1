using System;

namespace bombersquad_ai
{
	public class Action
	{
		
		public enum ActionType { NORTH, SOUTH, EAST, WEST, BOMB };
		
		private ActionType type;
		public ActionType ActionName {
			get { return type; }
		}
		
		public Action (ActionType type)
		{
			this.type = type;
		}		
		
		public override string ToString ()
		{
			return string.Format ("[Action: ActionName={0}]", type);
		}
	}
}

