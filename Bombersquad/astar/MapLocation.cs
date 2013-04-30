using System;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	namespace astar
	{
		public class MapLocation : Node
		{
			private Coords coords;
	
			public MapLocation (Coords coords)
			{
				this.coords = coords;
			}
	
			public Object getData ()
			{
				return this.coords;
			}
	
			public override bool Equals (Object obj)
			{
				bool result = (obj is MapLocation);
				if (!result) {
					return false;
				}
		
				MapLocation ml = (MapLocation)obj;
		
				return this.coords.Equals (ml.coords);
			}
	
			public override int GetHashCode ()
			{
				return this.coords.GetHashCode ();
			}
	
			public override string ToString ()
			{
				return this.coords.ToString ();
			}

		}
	}
}

