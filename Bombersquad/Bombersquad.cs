using System;
using System.Threading;
using bombersquad_ai.astar;
using System.Collections.Generic;

namespace bombersquad_ai
{
	public class Bombersquad
	{
		private SquadAI ai;
		private GameState gs;
		private ConsoleCanvas gfx;
		
		private TimeSpan loopTime; //milliseconds
		
		public Bombersquad (GameState gs)
		{
			this.gs = gs;
			this.gfx = new ConsoleCanvas();
			this.loopTime = new TimeSpan(0, 0, 0, 0, 100); //(0, 0, 0, 0, 40);
			
		}
		
		public void SetAI(SquadAI ai) {
			this.ai = ai;
		}

        public GameState getGameState() { return this.gs; }
        public TimeSpan getLoopTime() { return this.loopTime; }
        public SquadAI getAI() { return this.ai; }

		public void updateGame()
        {
			//while (!this.gs.GameOver) {
            if(!this.gs.GameOver) {
				DateTime begin = DateTime.Now;
				
				//Action pa = GetPlayerAction();
				//Action[] bots = this.ai.GetActions(this.gs);
				
				this.gs.UpdateGame(null, new Action[4]);

                Console.WriteLine("BLAH");
				DateTime end = DateTime.Now;
				
				TimeSpan diff = end - begin;
				
				if (diff < this.loopTime) {
					TimeSpan wait = this.loopTime - diff;
					Thread.Sleep(wait);
				}
			}
		}

        public void drawGameConsole()
        {
            this.gfx.DrawGameState(this.gs);
        }
		
		public Action GetPlayerAction() {
			/*if (Console.KeyAvailable) {
				ConsoleKeyInfo keyInfo = Console.ReadKey();
				ConsoleKey key = keyInfo.Key;
				if (key.Equals (ConsoleKey.UpArrow)) {
					return new Action(Action.ActionType.NORTH);
				} else if (key.Equals(ConsoleKey.LeftArrow)) {
					return new Action(Action.ActionType.WEST);
				}  else if (key.Equals(ConsoleKey.RightArrow)) {
					return new Action(Action.ActionType.EAST);
				}  else if (key.Equals(ConsoleKey.DownArrow)) {
					return new Action(Action.ActionType.SOUTH);
				} else if (key.Equals(ConsoleKey.Spacebar)) {
					return new Action(Action.ActionType.BOMB);
				}
			}*/
				
			return null;
		}
	}
}

