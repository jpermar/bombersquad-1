using System;
using bombersquad_ai.astar;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace astar
	{
		public class PathPlan
		{
			private static readonly double MOVEMENT_COST = 1;
			private GameState gs;

			public PathPlan (GameState gs)
			{
				this.gs = gs;
			}
			
			public Graph mapQuantization (bool isBombPassable, bool isDestructiblePassable)
			{
				WorldGraph graph = new WorldGraph (gs.Width * gs.Height);
				LocationData[,] map = this.gs.Map;
				for (int i = 0; i < gs.Width; i++) {
					for (int j = 0; j < gs.Height; j++) {
						Coords coords = Coords.coordsXY(i, j, gs.Width, gs.Height);
						int tileNum = coords.getTileNum ();
						List<Connection> connections = getLegalConnections (map,
					tileNum, isBombPassable, isDestructiblePassable);
						graph.addConnections (tileNum, connections);
					}
				}

				return graph;
			}

			private List<Connection> getLegalConnections (
			LocationData[,] map, int tileFrom,
			bool isBombPassable, bool isDestructiblePassable)
			{
				List<Connection> connections = new List<Connection> ();

				int mapWidth = this.gs.Width;
				int mapHeight = this.gs.Height;

				Coords coords = Coords.coordsTileNum (mapWidth, mapHeight, tileFrom);

				List<Coords> possibleCoords = this.gs.GetAdjacentAccessibleTiles (
				tileFrom, isBombPassable, isDestructiblePassable);

				foreach (Coords possibleCoord in possibleCoords) {
					Node from = new MapLocation (coords);
					Node to = new MapLocation (possibleCoord);
					Connection conn = new Connection (from, to, MOVEMENT_COST + this.gs.tacticalCost(possibleCoord));
					connections.Add (conn);
				}

				return connections;

			}

			public List<Coords> localization (List<Connection> path)
			{
				List<Coords> coordsList = new List<Coords> ();

				foreach (Connection conn in path) {
					MapLocation toNode = (MapLocation)conn.getToNode ();
					coordsList.Add ((Coords)toNode.getData ());
				}

				return coordsList;
			}

			/**
	 * 
	 * @param from
	 *            unit's current location
	 * @param to
	 *            the destination
	 * @param isHillPassable
	 *            true if unit can fly, otherwise false
	 * @return a valid plan for the unit to follow (barring movement barriers),
	 *         or an empty list of coords if no valid plan is found
	 */
			public List<Coords> calculateMovementPlan (Graph g, Coords from, Coords to)
			{

				// run A*
				//Console.WriteLine("Calculating path from " + from + " to " + to);
				AStar astar = new AStar ();
				MapLocation start = new MapLocation (from);
				/*
				List<Connection> conns = g.getConnections(start);
				foreach (Connection conn in conns) {
					Console.WriteLine("conn from start: " + conn);
					List<Connection> nextConns = g.getConnections(conn.getToNode());
					foreach(Connection c2 in nextConns) {
						Console.WriteLine("second level connections " + c2);
					}
				}
				*/
				MapLocation goal = new MapLocation (to);
				ManhattanHeuristic heuristic = new ManhattanHeuristic (goal);
				List<Connection> path = astar.pathfindAStar (g, start, goal, heuristic);
				// localize plan
				List<Coords> coords = new List<Coords> ();
				if (path != null) {
					coords = localization (path);
					/*
					Console.Write ("Found path: ");
					foreach(Coords coord in coords) {
						Console.Write (coord);
					}
					Console.WriteLine();
					*/
				} else {
					Console.WriteLine("no path found");
				}

				return coords;
			}
		}
	}
}

