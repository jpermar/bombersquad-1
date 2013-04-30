using System;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	namespace util {
		public class MovementUtil
		{
			
			public static Action getMovementActionType (Coords from, Coords to)
			{
				if (to.getX () == from.getX () + 1) {
					return new Action (Action.ActionType.EAST);
				} else if (to.getX () == from.getX () - 1) {
					return new Action (Action.ActionType.WEST);
				} else if (to.getY () == from.getY () + 1) {
					return new Action (Action.ActionType.SOUTH);
				} else {
					return new Action (Action.ActionType.NORTH);
				}
			}
			
			public static int manhattanDist(Coords from, Coords to) {
				int distance = Math.Abs (from.getX () - to.getX ()) + Math.Abs (from.getY () - to.getY ());
				return distance;
			}
		}
	}
}

