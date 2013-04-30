using System;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace astar
	{
		public interface Graph
		{
			List<Connection> getConnections (Node from);

			int getNumNodes ();
		}
	}
}

