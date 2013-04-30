using System;
using bombersquad_ai.fsm;
using bombersquad_ai.fsm.states;
using bombersquad_ai.astar;
using bombersquad_ai.decision;

namespace bombersquad_ai
{
	namespace decision
	{
		public class BomberAgent
		{
			private FSM<BomberAgent> suicideBomberFSM;
			private FSM<BomberAgent> safeBomberFSM;
			private FSM<BomberAgent> blockEscapeFSM;
			private FSM<BomberAgent> guardWallsFSM;
			private GameState gs;
			private bool isBombPassable;
			private int agentId;
			private ChokePoints chokepoints;

			public int AgentId {
				get { return agentId; }
			}
			
			public BomberAgent (GameState gs, bool isBombPassable, int agentID)
			{
				this.gs = gs;
				this.isBombPassable = isBombPassable;
				this.agentId = agentID;
				this.chokepoints = new ChokePoints(this.gs, this.isBombPassable);
				this.suicideBomberFSM = new FSM<BomberAgent> (this, null, null);
				State<BomberAgent> state = new PursueState (this.gs, this.isBombPassable);
				this.suicideBomberFSM.changeState (state);

				this.safeBomberFSM = new FSM<BomberAgent> (this, null, null);
				state = new PursueState (this.gs, this.isBombPassable);
				this.safeBomberFSM.changeState (state);

				this.blockEscapeFSM = new FSM<BomberAgent> (this, null, null);
				state = new PursueState (this.gs, this.isBombPassable);
				this.blockEscapeFSM.changeState (state);

				this.guardWallsFSM = new FSM<BomberAgent> (this, null, null);
				//state = new PredictDestinationState(this.gs, this.isBombPassable);
				state = new SurroundState(this.gs, this.isBombPassable);
				this.guardWallsFSM.changeState (state);
			}
			
			public Action getAction ()
			{
				
				//Action move = defaultFSM();
				Action move = null;
				if (this.agentId == 0) {
					move = suicideBombMove (this.suicideBomberFSM);
				} else if (this.agentId == 1) {
					move = safeBombMove (this.safeBomberFSM);
				} else if (this.agentId == 2) {
					move = blockEscapeMove (this.blockEscapeFSM);
				} else if (this.agentId == 3) {
					//move  = guardWallsMove (this.guardWallsFSM);
					move  = defaultFSM (this.guardWallsFSM);
					
				}
				return move;
			}
			
			/*
			private Action defaultFSM() {
				State<BomberAgent> current = this.fsm.getCurrentState ();
				
				Action move = null;
				//check if blowing up stuff and we need to evade bombs!!
				//if (current is BlastWallState) {
					//for (int i = 0; i < this.gs.NumberAIBombermen; i++) {
						Coords botCoords = this.gs.GetAgentCoords (this.agentId + 1);
						bool shouldMove = this.gs.isOnExplosionPath (botCoords);
						if (shouldMove) {
							State<BomberAgent> newState = new EvadeExplosionState (this.gs);
							Console.WriteLine ("Agent " + this.agentId + " switching to EvadeExplosionState");
							this.fsm.changeState (newState);
							move = this.fsm.getCurrentState ().getAction (this);
							Console.WriteLine ("Agent " + this.agentId + ", state " + this.fsm.getCurrentState().GetType().ToString() + ", move " + move);
							return move;
						}
					//}
				//}
			
				if (current.fail (this)) {
					Console.WriteLine ("Failed strategy " + current.GetType ());
					if (current is SurroundState) {
						//Chaaarrge!! Blow our way to the player!!!
						Console.WriteLine ("Switching to BlastWallState");
						BlastWallState newState = new BlastWallState (this.gs, false);
						this.fsm.changeState (newState);
					} else if (current is BombState) {
						Console.WriteLine ("Switching to SurroundState");
						//this.current = new Surround(this.gs, this.surroundIgnoreBombs);
						//this.current = new Surround(this.gs, this.isBombPassable);
						SurroundState newState = new SurroundState (this.gs, this.isBombPassable);
						this.fsm.changeState (newState);
					} else if (current is BlastWallState) {
						//map fail.... ruh roh
						Console.WriteLine ("AI can't move... map fail");
						//switch to BombState as last-ditch resort...
						Console.WriteLine ("Switching to BombState");
						BombState newState = new BombState (this.gs);
						this.fsm.changeState (newState);
					}
				}
			
				if (current.complete (this)) {
					Console.WriteLine ("Agent " + this.agentId + " completed strategy " + current.GetType ());
					if (current is SurroundState) {
						//this.current = new KillPlayer(this.gs);
						Console.WriteLine ("Switching to BombState");
						BombState newState = new BombState (this.gs);
						this.fsm.changeState (newState);
					} else {
						//this.current = new Surround(this.gs, this.surroundIgnoreBombs);
						//this.current = new Surround(this.gs, this.isBombPassable);
						SurroundState newState = new SurroundState (this.gs, this.isBombPassable);
						Console.WriteLine ("Agent " + this.agentId + " switching to SurroundState");
						this.fsm.changeState (newState);
					}
				}
			
				//this.fsm.getCurrentState().execute(this);
				//return this.current;
				move = this.fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + this.fsm.getCurrentState().GetType().ToString() + ", move " + move);
				return move;
			}
			*/
			
			private Action suicideBombMove (FSM<BomberAgent> fsm)
			{
				State<BomberAgent> current = fsm.getCurrentState ();
				
				Action move = null;
				State<BomberAgent> nextState = null;
				
				if (current is PursueState) {
					if (current.fail (this)) {
						nextState = new BlastWallState (this.gs, false);		
					} else if (current.complete (this)) {
						nextState = new BombState (this.gs);
					}
				} else if (current is BlastWallState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new PursueState (this.gs, this.isBombPassable);
					}
				} else if (current is BombState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new PursueState (this.gs, this.isBombPassable);
					}
				}
				
				if (nextState != null) {
					Console.WriteLine ("Agent " + this.agentId + " switching to " + nextState.GetType ().ToString ());
					fsm.changeState (nextState);
				}
				move = fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState ().GetType ().ToString () + ", move " + move);
				return move;
				
			}
			
			private Action safeBombMove (FSM<BomberAgent> fsm)
			{
				State<BomberAgent> current = fsm.getCurrentState ();
				
				Action move = null;
				State<BomberAgent> nextState = null;
				
				if (current is PursueState) {
					if (current.fail (this)) {
						nextState = new BlastWallState (this.gs, false);		
					} else if (current.complete (this)) {
						nextState = new BombState (this.gs);
					}
				} else if (current is BlastWallState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is BombState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is EvadeExplosionState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new PursueState(this.gs, this.isBombPassable);
					}
				}
				
				if (nextState != null) {
					Console.WriteLine ("Agent " + this.agentId + " switching to " + nextState.GetType ().ToString ());
					fsm.changeState (nextState);
				}
				move = fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState ().GetType ().ToString () + ", move " + move);
				return move;
			}
			
			private Action blockEscapeMove (FSM<BomberAgent> fsm)
			{
				State<BomberAgent> current = fsm.getCurrentState ();
				
				Action move = null;
				State<BomberAgent> nextState = null;
				
				if (current is PursueState) {
					if (current.fail (this)) {
						nextState = new BlastWallState (this.gs, false);
					}
					Coords choke = this.chokepoints.findChokePoint(this.isBombPassable, this);
					if (choke != null) {
						nextState = new CutOffChokePointState(this.gs, this.isBombPassable, choke);
					}
				} else if (current is BlastWallState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is CutOffChokePointState) {
					//Coords choke = this.chokepoints.findChokePoint(this.isBombPassable, this);
					//if ((choke == null) || current.fail(this)) {
					if (current.fail (this)) {
						nextState = new PursueState(this.gs, this.isBombPassable);
					} else if (current.complete(this)) {
						nextState = new BombState(this.gs);
					}
				} else if (current is BombState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is EvadeExplosionState) {
					if (current.fail (this) || current.complete (this)) {
						Coords choke = this.chokepoints.findChokePoint(this.isBombPassable, this);
						if (choke == null) {
							nextState = new PursueState(this.gs, this.isBombPassable);
						} else {
							nextState = new CutOffChokePointState(this.gs, this.isBombPassable, choke);
						}
					}
				}
				
				if (nextState != null) {
					Console.WriteLine ("Agent " + this.agentId + " switching to " + nextState.GetType ().ToString ());
					fsm.changeState (nextState);
				}
				move = fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState ().GetType ().ToString () + ", move " + move);
				return move;
			}
			
			/*
			private Action guardWallsMove (FSM<BomberAgent> fsm)
			{
				State<BomberAgent> current = fsm.getCurrentState ();
				
				Action move = null;
				State<BomberAgent> nextState = null;
				
				if (current is PredictDestinationState) {
					if (current.fail (this)) {
						nextState = new BlastWallState (this.gs, false);
					} else if (current.complete(this)) {
						nextState = new BombState(this.gs);
					}
				} else if (current is BlastWallState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is BombState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new EvadeExplosionState(this.gs);
					}
				} else if (current is EvadeExplosionState) {
					if (current.fail (this) || current.complete (this)) {
						nextState = new PredictDestinationState(this.gs, this.isBombPassable);
					}
				}
				
				if (nextState != null) {
					Console.WriteLine ("Agent " + this.agentId + " switching to " + nextState.GetType ().ToString ());
					fsm.changeState (nextState);
				}
				move = fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState ().GetType ().ToString () + ", move " + move);
				return move;
			}
			*/
			
			private Action defaultFSM(FSM<BomberAgent> fsm) {
				State<BomberAgent> current = fsm.getCurrentState ();
				
				Action move = null;
				//check if blowing up stuff and we need to evade bombs!!
				//if (current is BlastWallState) {
					//for (int i = 0; i < this.gs.NumberAIBombermen; i++) {
						Coords botCoords = this.gs.GetAgentCoords (this.agentId + 1);
						bool shouldMove = this.gs.isOnExplosionPath (botCoords);
						if (shouldMove) {
							State<BomberAgent> newState = new EvadeExplosionState (this.gs);
							Console.WriteLine ("Agent " + this.agentId + " switching to EvadeExplosionState");
							fsm.changeState (newState);
							move = fsm.getCurrentState ().getAction (this);
							Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState().GetType().ToString() + ", move " + move);
							return move;
						}
					//}
				//}
			
				if (current.fail (this)) {
					Console.WriteLine ("Failed strategy " + current.GetType ());
					if (current is SurroundState) {
						//Chaaarrge!! Blow our way to the player!!!
						Console.WriteLine ("Switching to BlastWallState");
						BlastWallState newState = new BlastWallState (this.gs, false);
						fsm.changeState (newState);
					} else if (current is BombState) {
						Console.WriteLine ("Switching to SurroundState");
						//this.current = new Surround(this.gs, this.surroundIgnoreBombs);
						//this.current = new Surround(this.gs, this.isBombPassable);
						SurroundState newState = new SurroundState (this.gs, this.isBombPassable);
						fsm.changeState (newState);
					} else if (current is BlastWallState) {
						//map fail.... ruh roh
						Console.WriteLine ("AI can't move... map fail");
						//switch to BombState as last-ditch resort...
						Console.WriteLine ("Switching to BombState");
						BombState newState = new BombState (this.gs);
						fsm.changeState (newState);
					}
				}
			
				if (current.complete (this)) {
					Console.WriteLine ("Agent " + this.agentId + " completed strategy " + current.GetType ());
					if (current is SurroundState) {
						//this.current = new KillPlayer(this.gs);
						Console.WriteLine ("Switching to BombState");
						BombState newState = new BombState (this.gs);
						fsm.changeState (newState);
					} else {
						//this.current = new Surround(this.gs, this.surroundIgnoreBombs);
						//this.current = new Surround(this.gs, this.isBombPassable);
						SurroundState newState = new SurroundState (this.gs, this.isBombPassable);
						Console.WriteLine ("Agent " + this.agentId + " switching to SurroundState");
						fsm.changeState (newState);
					}
				}
			
				//fsm.getCurrentState().execute(this);
				//return this.current;
				move = fsm.getCurrentState ().getAction (this);
				Console.WriteLine ("Agent " + this.agentId + ", state " + fsm.getCurrentState().GetType().ToString() + ", move " + move);
				return move;
			}
			
		}
	}
}

