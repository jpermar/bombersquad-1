using System;
using bombersquad_ai.decision;
using bombersquad_ai.astar;
using bombersquad_ai.util;
using System.Collections.Generic;

namespace bombersquad_ai
{
	namespace fsm
	{
		namespace states
		{
			public class BlastWallState : State<BomberAgent>
			{
				
				private GameState gs;
				private bool isBombPassable;
				private bool getMovesCalled;
				//private bool allAICanPath;
				private bool canPath;
				private bool plantedBomb;
				private bool cannotExecuteStrategy;
				
				public BlastWallState (GameState gs, bool isBombPassable)
				{
					this.gs = gs;
					this.isBombPassable = isBombPassable;
					this.getMovesCalled = false;
					//this.allAICanPath = false;
					this.plantedBomb = false;
					this.canPath = false;
					this.cannotExecuteStrategy = false;
				}
				
				public void enterState (BomberAgent owner)
				{
				}
				
				public Action getAction (BomberAgent owner)
				{
					getMovesCalled = true;
			
					//this.allAICanPath = true;
			
					Coords player = this.gs.GetAgentCoords (0);
					//Action[] moves = new Action[this.gs.NumberAIBombermen];
					Action move = null;

					PathPlan pathPlan = new PathPlan (this.gs);
					//Graph g = pathPlan.mapQuantization (true, false); //always use true here... otherwise AI can get stuck repeatedly dropping bombs (and won't move due to bombs)
					Graph g = pathPlan.mapQuantization (this.isBombPassable, false); //always use true here... otherwise AI can get stuck repeatedly dropping bombs (and won't move due to bombs)

					//for (int i = 0; i < moves.Length; i++) {
				
					Coords aiBotCoords = this.gs.GetAgentCoords (owner.AgentId + 1);
				
					//1. Check if we have open path to player... if so, we're done for this bot
					List<Coords> path = pathPlan.calculateMovementPlan (g, aiBotCoords, player);
					if (path.Count > 0) {
						this.canPath = true;
						return null;
					}
				
					//this.allAICanPath = false;
					//2. If not done, check if we have path through destructible walls (should always have this given valid map)
					Coords wallCoords = calcWallToDestroy (aiBotCoords, player);
				
					//3. Path to the first destructible wall using regular pathing
					if (wallCoords != null) {
						Console.WriteLine ("BlastWallState pathing to wall " + wallCoords);
						path = pathPlan.calculateMovementPlan (g, aiBotCoords, wallCoords);
						if (path.Count > 0) {
							//4. Add first move on that path
							move = MovementUtil.getMovementActionType (aiBotCoords, path [0]);
							Console.WriteLine ("Found move " + move + " for ai bot " + owner.AgentId + " to blow up wall " + wallCoords);
						} else {
							move = new Action (Action.ActionType.BOMB);
							this.plantedBomb = true;
						}
					}
				
				
					//}
			
					return move;
				}
				/*
				public void execute (Agent owner)
				{
					
				}
				*/

				public void exitState (BomberAgent owner)
				{
				}
				
				public bool fail (BomberAgent owner)
				{
					return this.cannotExecuteStrategy;
				}
				
				public bool complete (BomberAgent owner)
				{
					return (getMovesCalled && (this.canPath || this.plantedBomb));
				}
				
				private Coords calcWallToDestroy (Coords from, Coords destination)
				{
					//path to destination, counting destructible walls as pathable
					PathPlan pathPlan = new PathPlan (this.gs);
					Graph g = pathPlan.mapQuantization (this.isBombPassable, true);
					List<Coords> path = pathPlan.calculateMovementPlan (g, from, destination);
			
					if ((path.Count == 0) && isBombPassable) {
						Console.WriteLine ("MAP FAIL!"); //TODO JDP ????
						this.cannotExecuteStrategy = true;
					}
			
					for (int i = 0; i < path.Count; i++) {
						//foreach (Coords coord in path) {
						Coords coord = path [i];
						//first destructible wall encountered on path is the target
						LocationData datum = this.gs.GetLocationData (coord);
						if (datum.HasDestructibleWall ()) {
							Console.WriteLine ("BlastWallState found wall to blow up: " + coord);
							//find previous coord
							if (i - 1 < 0) { //case where we can't move anywhere but we can blow up ourselves and a wall!
								return from;
							} else {
								Coords bombDropCoord = path [i - 1];
								return bombDropCoord;
							}
						}
				
					}
			
					return null;
				}
			}
		}
	}
}

