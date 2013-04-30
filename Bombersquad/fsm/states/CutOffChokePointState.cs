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
			public class CutOffChokePointState : State<BomberAgent>
			{
				
				private GameState gs;
				//private readonly int MANHATTAN_DIST = 5;
				//private Coords[] targets;
				private bool isBombPassable;
				private bool cannotExecuteStrategy;
				private Coords choke;
				private bool atChoke;
				private bool completedGetAction;
				
				public CutOffChokePointState (GameState gs, bool isBombPassable, Coords choke)
				{
					this.gs = gs;
					this.isBombPassable = isBombPassable;
					this.cannotExecuteStrategy = false;
					this.choke = choke;
					this.atChoke = false;
					this.completedGetAction = false;
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
				
				public Action getAction (BomberAgent owner)
				{
					Action move = null;
					this.completedGetAction = true;
			
					PathPlan pathPlan = new PathPlan (this.gs);
					Graph g = pathPlan.mapQuantization (this.isBombPassable, false);
					
					Coords aiPos = this.gs.GetAgentCoords (owner.AgentId + 1);
					if (aiPos.Equals (this.choke)) {
						this.atChoke = true;
						return null;
					}
					List<Coords> path = pathPlan.calculateMovementPlan (g, aiPos, this.choke);
					if (path.Count > 0) {
						move = MovementUtil.getMovementActionType (aiPos, path[0]);
					} else {
						Console.WriteLine ("Agent " + owner.AgentId + " cannot move to choke point");
						this.cannotExecuteStrategy = true;
					}
			
					return move;
				}
		
				public bool complete (BomberAgent owner)
				{
						return (this.completedGetAction && this.atChoke);
				}
		
				public bool fail (BomberAgent owner)
				{
					return this.cannotExecuteStrategy;
				}
							
			}
		}
	}
}

