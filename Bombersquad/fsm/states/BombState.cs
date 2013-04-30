using System;
using bombersquad_ai.fsm;
using bombersquad_ai.decision;

namespace bombersquad_ai
{
	namespace fsm
	{
		namespace states
		{
			public class BombState : State<BomberAgent>
			{
				
				private readonly int MANHATTAN_KILL_DIST = 4;
				private GameState gs;
				private bool getMovesCalled;
				
				public BombState (GameState gs)
				{
					this.gs = gs;
					this.getMovesCalled = false;
				}
				
				public void enterState (BomberAgent owner)
				{
				}
				
				public Action getAction (BomberAgent owner)
				{
					//for each AI that is "in range" (using MANHATTAN_KILL_DIST), drop bomb
					getMovesCalled = true;
			
					Action move = new Action (Action.ActionType.BOMB);
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
					return getMovesCalled;
				}
			}
		}
	}
}

