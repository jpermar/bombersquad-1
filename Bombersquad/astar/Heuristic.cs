using System;

namespace bombersquad_ai
{
	namespace astar
	{
		public abstract class Heuristic
		{
			protected Node goal;
	
			public Heuristic (Node goal)
			{
				this.goal = goal;
			}
	
			public abstract double estimate (Node node);
		}
	}
}

