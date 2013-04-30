using System;

namespace bombersquad_ai
{
	namespace astar
	{
		public class ManhattanHeuristic : Heuristic
		{
			public ManhattanHeuristic (Node goal) : base(goal)
			{
				//super (goal);
			}
	
			public override double estimate (Node node)
			{
				MapLocation from = (MapLocation)node;
				Coords fromCoords = (Coords)from.getData ();
				MapLocation goalLocation = (MapLocation)this.goal;
				Coords goalCoords = (Coords)goalLocation.getData ();
				double distance = Math.Abs (fromCoords.getX () - goalCoords.getX ()) + Math.Abs (fromCoords.getY () - goalCoords.getY ());
				return distance;
			}
		}
	}
}

