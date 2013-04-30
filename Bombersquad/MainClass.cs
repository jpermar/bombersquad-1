using System;
using System.Collections.Generic;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	class MainClass
	{
        // Alter all config data in GameConfig.cs
        int width = GameConfig.gameWidth;
        int height = GameConfig.gameHeight;

        public Bombersquad game;
        
        private TimeSpan detonationTime;
		
        GameState gs;
		
		public MainClass ()
		{
			this.detonationTime = new TimeSpan (0, 0, 0, 5, 0);
			this.gs = new GameState (width, height, this.detonationTime);
		}
				
		GameState GetGameState ()
		{
			return this.gs;
		}
		
		public void runOnce ()
		{
            
            //MainClass go = new MainClass ();
			
            // --- START temporary wall generation
            Coords coords = Coords.coordsXY (3, 3, width, height);
			initLocation (LocationData.Tile.INDESTRUCTIBLE_WALL, coords);
			
			coords = Coords.coordsXY (3, 4, width, height);
			initLocation (LocationData.Tile.INDESTRUCTIBLE_WALL, coords);
			coords = Coords.coordsXY (3, 5, width, height);
			initLocation (LocationData.Tile.INDESTRUCTIBLE_WALL, coords);

			coords = Coords.coordsXY (2, 3, width, height);
			initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);
			coords = Coords.coordsXY (4, 3, width, height);
			initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);
			coords = Coords.coordsXY (5, 3, width, height);
			initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);
			coords = Coords.coordsXY (6, 3, width, height);
			initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);
			coords = Coords.coordsXY (7, 3, width, height);
			initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);

            coords = Coords.coordsXY(width - 9, height - 2, width, height);
            initLocation(LocationData.Tile.DESTRUCTIBLE_WALL, coords);
            coords = Coords.coordsXY(width - 9, height - 1, width, height);
            initLocation(LocationData.Tile.DESTRUCTIBLE_WALL, coords);
            // --- END temporary wall generation
			
			// Spawn Player
            coords = Coords.coordsXY(GameConfig.playerStartX, GameConfig.playerStartY, width, height);
			initLocation (LocationData.Tile.PLAYER, coords);
			GetGameState ().InitBomberman (LocationData.Tile.PLAYER, coords);

			// Spawn Bomber AI 1 of 4
            coords = Coords.coordsXY(GameConfig.Bomber1StartX, GameConfig.Bomber1StartY, width, height);
			initLocation (LocationData.Tile.AI_1, coords);
			GetGameState ().InitBomberman (LocationData.Tile.AI_1, coords);

            // Spawn Bomber AI 2 of 4
            coords = Coords.coordsXY(GameConfig.Bomber2StartX, GameConfig.Bomber2StartY, width, height);
            initLocation(LocationData.Tile.AI_2, coords);
			GetGameState ().InitBomberman (LocationData.Tile.AI_2, coords);

            // Spawn Bomber AI 3 of 4
            coords = Coords.coordsXY(GameConfig.Bomber3StartX, GameConfig.Bomber3StartY, width, height);
            initLocation(LocationData.Tile.AI_3, coords);
			GetGameState ().InitBomberman (LocationData.Tile.AI_3, coords);

            // Spawn Bomber AI 4 of 4
            coords = Coords.coordsXY(GameConfig.Bomber4StartX, GameConfig.Bomber4StartY, width, height);
            initLocation(LocationData.Tile.AI_4, coords);
			GetGameState ().InitBomberman (LocationData.Tile.AI_4, coords);
			
			//add pen for AI...
			for (int i = 1; i < 10; i++) {
				coords = Coords.coordsXY (width - i, height - 3, width, height);
				initLocation (LocationData.Tile.DESTRUCTIBLE_WALL, coords);
			}
					
			this.game = new Bombersquad (GetGameState ());
			bool isBombPassable = false;
            SquadAI ai = new SquadAI(GetGameState(), isBombPassable);
            this.game.SetAI(ai);
		}

        public bool updateGame()
        {
            this.game.updateGame();
            return false;
        }

        public void drawGameConsole()
        {
            this.game.drawGameConsole();
        }
		
		void initLocation (LocationData.Tile tile, Coords coords)
		{
			List<LocationData.Tile> objects = new List<LocationData.Tile> ();
			objects.Add (tile);
			LocationData datum = new LocationData (objects);
			this.gs.SetLocationData (datum, coords);
			
		}
	}
}
