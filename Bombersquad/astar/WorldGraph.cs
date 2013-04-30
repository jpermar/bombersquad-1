using System;
using System.Collections.Generic;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	namespace astar
	{
		public class WorldGraph : Graph
		{
			private Dictionary<int, List<Connection>> connections;
			private int numNodes;
	
			public WorldGraph (int numNodes)
			{
				this.connections = new Dictionary<int, List<Connection>> ();
				this.numNodes = numNodes;
			}
	
			public void addConnections (int tileNum, List<Connection> connections)
			{
				this.connections.Add (tileNum, connections);
			}
	
			public List<Connection> getConnections (Node from)
			{
				MapLocation location = (MapLocation)from;
		
				Coords coords = (Coords)location.getData ();
				int tileNum = coords.getTileNum ();
		
				List<Connection> retrievedConnectList;
				if (this.connections.TryGetValue(tileNum, out retrievedConnectList)) {
					//return this.connections.get (tileNum);
					return retrievedConnectList;
				} else {
					String msg = "attempted to retrieve connection list from map, but couldn't find a list for tile " + tileNum;
					throw new Exception(msg);
				}
				
		
			}

			public int getNumNodes ()
			{
				return this.numNodes;
			}
		}
	}
}

