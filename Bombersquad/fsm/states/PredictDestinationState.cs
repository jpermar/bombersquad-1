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
			public class PredictDestinationState : State<BomberAgent>
			{
				
				private GameState gs;
				//private readonly int MANHATTAN_DIST = 5;
				//private Coords[] targets;
				private bool isBombPassable;
				private bool cannotExecuteStrategy;
				private bool atDestination;
				private bool completedGetAction;
				private Coords lastPlayerPosition;

				public Coords LastPlayerPosition {
					get { return this.lastPlayerPosition; }
				}
				
				public PredictDestinationState (GameState gs, bool isBombPassable)
				{
					this.gs = gs;
					this.isBombPassable = isBombPassable;
					this.cannotExecuteStrategy = false;
					this.completedGetAction = false;
					this.lastPlayerPosition = null;
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
				
				private Coords predictDestination ()
				{
					//want to find four locations... north, south, east, and west of Bomberman
					Coords playerCoords = this.gs.GetAgentCoords (0);
					List<Coords> visited = new List<Coords> ();
					visited.Add (playerCoords);
					Func<Coords, Coords, bool> test = null;
					if (playerCoords.isNorthOf (this.lastPlayerPosition)) {
						//find a destructible wall to the north
						test = delegate(Coords first, Coords second) {
							LocationData datum = this.gs.GetLocationData(first);
							return (first.isNorthOf (second) && (datum.HasDestructibleWall()));
						};
					} else if (playerCoords.isEastOf (this.lastPlayerPosition)) {
						test = delegate(Coords first, Coords second) {
							LocationData datum = this.gs.GetLocationData(first);
							return (first.isEastOf (second) && (datum.HasDestructibleWall()));
						};
					} else if (playerCoords.isSouthOf (this.lastPlayerPosition)) {
						test = delegate(Coords first, Coords second) {
							LocationData datum = this.gs.GetLocationData(first);
							return (first.isSouthOf (second) && (datum.HasDestructibleWall()));
						};
					} else if (playerCoords.isWestOf (this.lastPlayerPosition)) {
						test = delegate(Coords first, Coords second) {
							LocationData datum = this.gs.GetLocationData(first);
							return (first.isWestOf (second) && (datum.HasDestructibleWall()));
						};
					}
					
					//player may not have moved...
					if (test != null) {
						Coords greedyCoord = Searches.greedyCoordsSearch (this.gs, playerCoords, test, isBombPassable, visited);
						if (greedyCoord != null) {
						Console.WriteLine("found greedy coord " + greedyCoord);
							return greedyCoord;
						}
					}
					
					return null;
				}
				
				public Action getAction (BomberAgent owner)
				{
					
					if (this.lastPlayerPosition == null) {
						this.lastPlayerPosition = this.gs.GetAgentCoords (0);
						return null;
					}
					
					Action move = null;
					
					if (this.gs.GetAgentCoords(0).Equals (this.lastPlayerPosition)) {
						this.lastPlayerPosition = this.gs.GetAgentCoords(0);
						//player didn't move... blah...
						this.cannotExecuteStrategy = true;
						Console.WriteLine("player didn't move");
						return null;
					}
					Coords destination = predictDestination ();
					this.lastPlayerPosition = this.gs.GetAgentCoords(0);
					if (destination != null) {
						Console.WriteLine("have predicted destination");
						this.completedGetAction = true;
					} else {
						//this.cannotExecuteStrategy = true;
						return null; //chill until we have a destination
					}

					
					PathPlan pathPlan = new PathPlan (this.gs);
					Graph g = pathPlan.mapQuantization (this.isBombPassable, false);
					
					Coords aiPos = this.gs.GetAgentCoords (owner.AgentId + 1);
					if (aiPos.Equals (destination)) {
						this.atDestination = true;
						return null;
					}
					
					List<Coords> path = pathPlan.calculateMovementPlan (g, aiPos, destination);
					if (path.Count > 0) {
						move = MovementUtil.getMovementActionType (aiPos, path [0]);
					} else {
						Console.WriteLine ("Agent " + owner.AgentId + " cannot move to predicted destination");
						this.cannotExecuteStrategy = true;
					}
			
					return move;
				}
		
				public bool complete (BomberAgent owner)
				{
					return (this.completedGetAction && this.atDestination);
				}
		
				public bool fail (BomberAgent owner)
				{
					return this.cannotExecuteStrategy;
				}
							
			}
		}
	}
}

