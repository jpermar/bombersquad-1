using System;
using bombersquad_ai.astar;
using bombersquad_ai.decision;
using System.Collections.Generic;

namespace bombersquad_ai
{
	public class SquadAI
	{
		
		//private ChooseStrategy cs;
		private GameState gs;
		private BomberAgent[] agents;

		public SquadAI (GameState gs, bool isBombPassable)
		{
			//this.cs = cs;
			this.gs = gs;
			InitAgents(isBombPassable);
		}
		
		private void InitAgents(bool isBombPassable) {
			this.agents = new BomberAgent[gs.MAX_NUM_AI_BOMBERS];
			for (int i = 0; i < gs.MAX_NUM_AI_BOMBERS; i++) {
				PlayerState playerState = gs.GetPlayerState(i+1);
				if (playerState != null) {
					agents[i] = new BomberAgent(this.gs, isBombPassable, i);
				}
			}
		}
		
		public Action[] GetActions ()
		{
			
			Action[] moves = new Action[agents.Length];
			
			for (int i = 0; i < agents.Length; i++) {
				moves[i] = agents[i].getAction();
			}
			
			//Strategy strat = this.cs.determineStrat ();
			
			//Action[] moves = strat.GetMoves ();
			
			return moves;
		}
		
	}
}

