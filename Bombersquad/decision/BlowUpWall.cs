using System;
using bombersquad_ai.astar;
using bombersquad_ai.util;
using System.Collections.Generic;

namespace bombersquad_ai
{
	public class BlowUpWall : Strategy
	{
		
		private GameState gs;
		private bool isBombPassable;
		private bool getMovesCalled;
		private bool allAICanPath;
		private bool cannotExecuteStrategy;
		
		public BlowUpWall (GameState gs, bool isBombPassable)
		{
			this.gs = gs;
			this.isBombPassable = isBombPassable;
			this.getMovesCalled = false;
			this.allAICanPath = false;
			this.cannotExecuteStrategy = false;
		}
		
		//modified pathfinding... assume that agent can path through destructible walls
		//and simply calc path
		public Coords calcWallToDestroy (Coords from, Coords destination)
		{
			//path to destination, counting destructible walls as pathable
			PathPlan pathPlan = new PathPlan (this.gs);
			Graph g = pathPlan.mapQuantization (this.isBombPassable, true);
			List<Coords> path = pathPlan.calculateMovementPlan (g, from, destination);
			
			if ((path.Count == 0) && isBombPassable) {
				Console.WriteLine("MAP FAIL!"); //TODO JDP ????
				this.cannotExecuteStrategy = true;
			}
			
			for (int i = 0; i < path.Count; i++) {
			//foreach (Coords coord in path) {
				Coords coord = path[i];
				//first destructible wall encountered on path is the target
				LocationData datum = this.gs.GetLocationData (coord);
				if (datum.HasDestructibleWall ()) {
					Console.WriteLine("BlowUpWall found wall to blow up: " + coord);
					//find previous coord
					if (i-1 < 0) { //case where we can't move anywhere but we can blow up ourselves and a wall!
						return from;
					} else {
						Coords bombDropCoord = path[i-1];
						return bombDropCoord;
					}
				}
				
			}
			
			return null;
		}
		
		public Action[] GetMoves ()
		{
			getMovesCalled = true;
			
			this.allAICanPath = true;
			
			Coords player = this.gs.GetAgentCoords (0);
			Action[] moves = new Action[this.gs.NumberAIBombermen];

			PathPlan pathPlan = new PathPlan (this.gs);
			//Graph g = pathPlan.mapQuantization (true, false); //always use true here... otherwise AI can get stuck repeatedly dropping bombs (and won't move due to bombs)
			Graph g = pathPlan.mapQuantization (this.isBombPassable, false); //always use true here... otherwise AI can get stuck repeatedly dropping bombs (and won't move due to bombs)

			for (int i = 0; i < moves.Length; i++) {
				
				Coords aiBotCoords = this.gs.GetAgentCoords (i + 1);
				
				//1. Check if we have open path to player... if so, we're done for this bot
				List<Coords> path = pathPlan.calculateMovementPlan (g, aiBotCoords, player);
				if (path.Count > 0) {
					continue; //go to next bot...
				}
				
				this.allAICanPath = false;
				//2. If not done, check if we have path through destructible walls (should always have this given valid map)
				Coords wallCoords = calcWallToDestroy (aiBotCoords, player);
				
				//3. Path to the first destructible wall using regular pathing
				if (wallCoords != null) {
					Console.WriteLine("BlowUpWall pathing to wall " + wallCoords);
					path = pathPlan.calculateMovementPlan (g, aiBotCoords, wallCoords);
					if (path.Count > 0) {
						//4. Add first move on that path
						moves [i] = MovementUtil.getMovementActionType(aiBotCoords, path[0]);
						Console.WriteLine("Found move " + moves[i] + " for ai bot " + i+1 + " to blow up wall " + wallCoords);
					} else {
						moves[i] = new Action(Action.ActionType.BOMB);
					}
				}
				
				
			}
			
			return moves;
		}
		
		public bool IsStratExecuted ()
		{
			return (getMovesCalled && allAICanPath);
		}
		
		public bool CannotExecute() {
			return this.cannotExecuteStrategy;
		}
	}
}

