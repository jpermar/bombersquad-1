using System;
using bombersquad_ai.fsm;
using bombersquad_ai.astar;
using bombersquad_ai.decision;
using bombersquad_ai.util;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace fsm
	{
		namespace states
		{
			public class PursueState : State<BomberAgent>
			{
				
				private GameState gs;
				//private readonly int MANHATTAN_DIST = 5;
				//private Coords[] targets;
				private Coords target;
				private bool isBombPassable;
				private bool cannotExecuteStrategy;
				
				public PursueState (GameState gs, bool isBombPassable)
				{
					this.gs = gs;
					this.isBombPassable = isBombPassable;
					//this.targets = new Coords[this.gs.NumberAIBombermen];
					this.cannotExecuteStrategy = false;
				}
				
				public void enterState (BomberAgent owner)
				{
				}
				
				/*
				public void execute (Agent owner)
				{
					
				}
				*/

				public void exitState (BomberAgent owner)
				{
				}
				
				public List<Coords> calcSurroundLocations (bool isBombPassable)
				{
					List<Coords> surroundCoords = new List<Coords> ();
			
					//want to find four locations... north, south, east, and west of Bomberman
					Coords playerCoords = this.gs.GetAgentCoords (0);

					bool haveNorthCoord = false;
					bool haveSouthCoord = false;
					bool haveWestCoord = false;
					bool haveEastCoord = false;
			
					Coords northCoord = null;
					Coords southCoord = null;
					Coords westCoord = null;
					Coords eastCoord = null;
			
					List<Coords> visited = new List<Coords> ();
					visited.Add (playerCoords);
			
					if (!haveNorthCoord) {
						//want to find a path north from where bomberman is and block it off...
						Func<Coords, Coords, bool> isNorth = delegate(Coords first, Coords second) {
							return first.isNorthOf (second);
						};
						Coords greedyCoord = Searches.greedyCoordsSearch (this.gs, playerCoords, isNorth, isBombPassable, visited);
						if (greedyCoord != null) {
							northCoord = greedyCoord;
							surroundCoords.Add (northCoord);
							visited.Add (northCoord);
						}
					}

					if (!haveSouthCoord) {
						//want to find a path north from where bomberman is and block it off...
						Func<Coords, Coords, bool> isSouth = delegate(Coords first, Coords second) {
							return first.isSouthOf (second);
						};
						Coords greedyCoord = Searches.greedyCoordsSearch (this.gs, playerCoords, isSouth, isBombPassable, visited);
						if (greedyCoord != null) {
							southCoord = greedyCoord;
							surroundCoords.Add (southCoord);
							visited.Add (southCoord);
						}
					}

					if (!haveWestCoord) {
						//want to find a path north from where bomberman is and block it off...
						Func<Coords, Coords, bool> isWest = delegate(Coords first, Coords second) {
							return first.isWestOf (second);
						};
						Coords greedyCoord = Searches.greedyCoordsSearch (this.gs, playerCoords, isWest, isBombPassable, visited);
						if (greedyCoord != null) {
							westCoord = greedyCoord;
							surroundCoords.Add (westCoord);
							visited.Add (westCoord);
						}
					}

					if (!haveEastCoord) {
						//want to find a path north from where bomberman is and block it off...
						Func<Coords, Coords, bool> isEast = delegate(Coords first, Coords second) {
							return first.isEastOf (second);
						};
						Coords greedyCoord = Searches.greedyCoordsSearch (this.gs, playerCoords, isEast, isBombPassable, visited);
						if (greedyCoord != null) {
							eastCoord = greedyCoord;
							surroundCoords.Add (eastCoord);
							visited.Add (eastCoord);
						}
					}

					return surroundCoords;	
				}
		
				public Action getAction (BomberAgent owner)
				{
					//surround moves...
					//List<Coords> surroundCoords = calcSurroundLocations (this.isBombPassable);
			
					Action move = null;
			
					PathPlan pathPlan = new PathPlan (this.gs);
					Graph g = pathPlan.mapQuantization (this.isBombPassable, false);
					
					/*
					if (surroundCoords.Count == 0) {
						Console.WriteLine ("No surround coords found");
						this.cannotExecuteStrategy = true;
						return null;
					}
					*/
			
					//int i = 0;
					
					//may not be enough surround coords for all agents...
					/*
					if (owner.AgentId > surroundCoords.Count-1) {
						return null;
					}
					*/
					
					Coords playerCoord = this.gs.GetAgentCoords(0); //player
					List<Coords> adj = this.gs.GetAdjacentAccessibleTiles(playerCoord.getTileNum(), this.isBombPassable, false);
					
					if (adj.Count == 0) { //should never happen
						return null;
					}
					
					Random r = new Random();
					int chosenIndex = (int)Math.Floor(r.NextDouble() * adj.Count);
					
					this.target = adj[chosenIndex];
					
					
					
					//foreach (Coords coord in surroundCoords) {
					//this.targets [i] = coord;
					//this.target = playerCoord;
					Coords aiPos = this.gs.GetAgentCoords (owner.AgentId + 1);
					List<Coords> path = pathPlan.calculateMovementPlan (g, aiPos, this.target);
					if (path.Count > 0) {
						move = MovementUtil.getMovementActionType (aiPos, path[0]);
					} else {
						Console.WriteLine ("Agent " + owner.AgentId + " cannot pursue");
						this.cannotExecuteStrategy = true;
					}
					//i++;
					//}
			
					return move;
				}
		
				public bool complete (BomberAgent owner)
				{
					bool executed = true;
			
					bool allNull = true;
			
					//check if this agent is "close to" player
					//for (int i = 0; i < this.targets.Length; i++) {
						//Coords dest = this.targets [i];
						if (this.target != null) {
							allNull = false;
							Coords aiPos = this.gs.GetAgentCoords (owner.AgentId + 1);
							int distance = MovementUtil.manhattanDist (aiPos, this.target);
							if (distance > this.gs.EXPLOSION_RADIUS) {
								executed = false;
							}
						}
					
					//}
			
					if (allNull) {
						return false;
					}
			
					return executed;
				}
		
				public bool fail (BomberAgent owner)
				{
					return this.cannotExecuteStrategy;
				}
			}
		}
	}
}

