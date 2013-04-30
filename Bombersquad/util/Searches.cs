using System;
using bombersquad_ai.astar;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace util {

		public class Searches
		{
		
			//Greedy BFS starting at "from" Coords
			//Returns null or a Coords that satisfies the test
			public static Coords greedyCoordsSearch(GameState gs, Coords from, Func<Coords, Coords, bool> test, bool isBombPassable, List<Coords> visited) {
				
				List<Coords> allCoords = new List<Coords>();
				
				List<Coords> adjCoords = gs.GetAdjacentAccessibleTiles(from.getTileNum(), isBombPassable, false);
				
				if (adjCoords.Count == 0) { return null; }
				
				allCoords.AddRange(adjCoords);
				
				while (allCoords.Count > 0) {
					Coords coord = allCoords[0];
					//Console.WriteLine("removing " + coord);
					allCoords.Remove(coord);
					visited.Add(coord);
					if (test(coord, from)) {
						return coord;
					} else {
						List<Coords> newCoords = gs.GetAdjacentAccessibleTiles(coord.getTileNum(), isBombPassable, false);
						//Console.WriteLine("adding " + newCoords);
						foreach(Coords potentialCoord in newCoords) {
							if (!visited.Contains(potentialCoord) && !allCoords.Contains(potentialCoord)) {
								allCoords.Add(potentialCoord);
							}
						}
					}
				}
				
				return null;
			}
		}
	}
}

