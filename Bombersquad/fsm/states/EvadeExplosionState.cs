using System;
using bombersquad_ai.fsm;
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
			public class EvadeExplosionState : State<BomberAgent>
			{
				
				private bool getMovesCalled;
				private bool allSafe;
				private GameState gs;
				
				public EvadeExplosionState (GameState gs)
				{
					this.getMovesCalled = false;
					this.allSafe = false;
					this.gs = gs;
				}
				
				public void enterState (BomberAgent owner)
				{
				}
				
				public Action getAction (BomberAgent owner)
				{
					getMovesCalled = true;
					this.allSafe = true;
			
					Action move = null;
			
					//for (int i = 0; i < moves.Length; i++) {
					Coords botCoords = this.gs.GetAgentCoords (owner.AgentId + 1);
					bool shouldMove = this.gs.isOnExplosionPath (botCoords);
					if (shouldMove) {
						this.allSafe = false;
						//find a safe spot to move to...
						List<Coords> visited = new List<Coords> ();
						visited.Add (botCoords);
						Func<Coords, Coords, bool> isSafe = delegate(Coords coords, Coords from) {
							return (!this.gs.isOnExplosionPath (coords));
						};
						Coords safeCoords = Searches.greedyCoordsSearch (this.gs, botCoords, isSafe, true, visited); //isBombPassable true in case we're surrounded... search for a safe location nearby!
						if (safeCoords != null) {
							//move there...
							PathPlan pathPlan = new PathPlan (this.gs);
							Graph g = pathPlan.mapQuantization (true, false);
							List<Coords> path = pathPlan.calculateMovementPlan (g, botCoords, safeCoords);
							if (path.Count > 0) {
								Coords[] pathCoords = path.ToArray ();
								move = MovementUtil.getMovementActionType (botCoords, pathCoords [0]);
							} else {
								//C'est la vie!
							}
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
					return false;
				}
				
				public bool complete (BomberAgent owner)
				{
					return (this.getMovesCalled && this.allSafe);
				}
			}
		}
	}
}

