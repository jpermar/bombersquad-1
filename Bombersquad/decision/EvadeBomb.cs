using System;
using System.Collections.Generic;
using bombersquad_ai.astar;
using bombersquad_ai.util;

namespace bombersquad_ai
{
	public class EvadeBomb : Strategy
	{
		private bool getMovesCalled;
		private bool allSafe;
		private GameState gs;

		public EvadeBomb (GameState gs)
		{
			this.getMovesCalled = false;
			this.allSafe = false;
			this.gs = gs;
		}
		
		public Action[] GetMoves ()
		{
			getMovesCalled = true;
			this.allSafe = true;
			
			Action[] moves = new Action[this.gs.NumberAIBombermen];
			
			for (int i = 0; i < moves.Length; i++) {
				Coords botCoords = this.gs.GetAgentCoords (i + 1);
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
							Action move = MovementUtil.getMovementActionType (botCoords, pathCoords [0]);
							moves [i] = move;
						} else {
							//C'est la vie!
						}
					}
				}
			}
			
			return moves;
		}
		
		public bool IsStratExecuted ()
		{
			return (this.getMovesCalled && this.allSafe);
		}
		
		public bool CannotExecute ()
		{
			return false;
		}
	}
}

