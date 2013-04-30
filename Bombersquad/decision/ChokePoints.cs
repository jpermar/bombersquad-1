using System;
using bombersquad_ai.fsm;
using bombersquad_ai.astar;
using bombersquad_ai.decision;
using bombersquad_ai.util;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace decision
	{
		public class ChokePoints
		{
		
			private GameState gs;
			private bool isBombPassable;
		
			public ChokePoints (GameState gs, bool isBombPassable)
			{
				this.gs = gs;
				this.isBombPassable = isBombPassable;
			}
		
			public Coords findChokePoint (bool isBombPassable, BomberAgent owner)
			{
					
				//calc path from player to AI agent
				//walk the path, looking for any "choke point" tiles, defined as
				//a tile with exactly 2 adjacent accessible points, and those two are the previous
				//and next coords on the path
				PathPlan pathPlan = new PathPlan (this.gs);
				Graph g = pathPlan.mapQuantization (this.isBombPassable, false);
				Coords playerCoord = this.gs.GetAgentCoords (0); //player
					
				Coords aiCoord = this.gs.GetAgentCoords (owner.AgentId + 1);
				List<Coords> path = pathPlan.calculateMovementPlan (g, playerCoord, aiCoord);
				if (path.Count > 2) { //need path of length 3 to identify choke points
						
					//found a path, so test for choke point
					for (int i = 1; i < path.Count-1; i++) {
						Coords coord = path [i];
						List<Coords> adj = this.gs.GetAdjacentAccessibleTiles (coord.getTileNum (), this.isBombPassable, false);
						if (adj.Count != 2) {
							continue;
						}
						
						Console.WriteLine("found candidate..." + coord);
						
						bool found = true;
						if ((!adj [0].Equals (path [i - 1]) && (!adj [0].Equals (path [i + 1])))) {
							found = false;
						}
						if ((!adj [1].Equals (path [i - 1]) && (!adj [1].Equals (path [i + 1])))) {
							found = false;
						}
						if (found) {
							Console.WriteLine("found choke point!!");
							return coord;
						}
							
					}
						
				}
					
				return null;
			}
		}
	}
}

