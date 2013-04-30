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
        int numWallsRegular = 50;
        int numWallsIndestructible = 0;
        public Bombersquad game;

        private TimeSpan detonationTime;

        GameState gs;

        public MainClass()
        {
            this.detonationTime = new TimeSpan(0, 0, 0, 5, 0);
            this.gs = new GameState(width, height, this.detonationTime);
        }

        GameState GetGameState()
        {
            return this.gs;
        }

        public void runOnce()
        {
            generateLevel();
            //MainClass go = new MainClass ();

            // --- START temporary wall generation
            /*Coords coords = Coords.coordsXY (3, 3, width, height);
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

            Random rand = new Random();

            for (int i = 0; i < 50; i++)
            {
                coords = Coords.coordsXY(rand.Next(4, 17), rand.Next(4, 17), width, height);
                initLocation(LocationData.Tile.DESTRUCTIBLE_WALL, coords);
            }

            for (int i = 0; i < 6; i++)
            {
                coords = Coords.coordsXY(rand.Next(4, 17), rand.Next(4, 17), width, height);
                initLocation(LocationData.Tile.INDESTRUCTIBLE_WALL, coords);
            }

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
			}*/

            this.game = new Bombersquad(GetGameState());
            bool isBombPassable = false;
            SquadAI ai = new SquadAI(GetGameState(), isBombPassable);
            this.game.SetAI(ai);
        }

        public void generateLevel()
        {
            placeUnbreakables();
            placeBreakables();
            placeAgents();
        }

        public void placeUnbreakables()
        {
            Random rand = new Random();
            Coords coords;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (i == 0 || i == width - 1 || j == 0 || j == height - 1)
                    {
                        coords = Coords.coordsXY(i, j, width, height);
                        initLocation(LocationData.Tile.INDESTRUCTIBLE_WALL, coords);
                    }
                    else
                    {

                        if (rand.Next(0, 101) < 15 && numWallsIndestructible <= 30)
                        {
                            coords = Coords.coordsXY(i, j, width, height);
                            initLocation(LocationData.Tile.INDESTRUCTIBLE_WALL, coords);
                        }
                    }
                }
            }
        }

        public void placeAgents()
        {
            List<Coords> list = new List<Coords>();
            for (int i = 2; i < width - 2; i++)
            {
                for (int j = 2; j < height - 2; j++)
                {
                    list.Add(Coords.coordsXY(i, j, width, height));
                }
            }
            Random rand = new Random();
            int k = rand.Next(0, list.Count);
            Coords player = list[k];
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY(), width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY(), width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX(), player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX(), player.getY() + 1, width, height)).Objects.Clear();
            initLocation(LocationData.Tile.PLAYER, player);
            GetGameState().InitBomberman(LocationData.Tile.PLAYER, player);
            k = rand.Next(0, list.Count);
            player = list[k];
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY(), width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY(), width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX(), player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX(), player.getY() + 1, width, height)).Objects.Clear();
            initLocation(LocationData.Tile.AI_1, player);
            GetGameState().InitBomberman(LocationData.Tile.AI_1, player);
            k = rand.Next(0, list.Count);
            player = list[k];
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY(), width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY(), width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX(), player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX(), player.getY() + 1, width, height)).Objects.Clear();
            initLocation(LocationData.Tile.AI_2, player);
            GetGameState().InitBomberman(LocationData.Tile.AI_2, player);
            k = rand.Next(0, list.Count);
            player = list[k];
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY(), width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY(), width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX(), player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX(), player.getY() + 1, width, height)).Objects.Clear();
            initLocation(LocationData.Tile.AI_3, player);
            GetGameState().InitBomberman(LocationData.Tile.AI_3, player);
            k = rand.Next(0, list.Count);
            player = list[k];
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY(), width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY(), width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX() - 1, player.getY() + 1, width, height)).Objects.Clear();
            list.Remove(Coords.coordsXY(player.getX(), player.getY() + 1, width, height));
            GetGameState().GetLocationData(Coords.coordsXY(player.getX(), player.getY() + 1, width, height)).Objects.Clear();
            initLocation(LocationData.Tile.AI_4, player);
            GetGameState().InitBomberman(LocationData.Tile.AI_4, player);
        }


        public void placeBreakables()
        {
            Coords coords;
            Random rand = new Random();
            /*
            for (int i = 1; i < width - 1; i += 2)
            {
                for (int j = 1; j < height - 1; j += 2)
                {
                    coords = Coords.coordsXY(i, j, width, height);
                    LocationData loc = GetGameState().GetLocationData(coords);
                    if (loc.Objects.Count == 0)
                    {
                        Coords left = Coords.coordsXY(i - 1, j, width, height);
                        Coords right = Coords.coordsXY(i + 1, j, width, height);
                        Coords up = Coords.coordsXY(i, j - 1, width, height);
                        Coords down = Coords.coordsXY(i - 1, j + 1, width, height);
                        if (GetGameState().GetLocationData(left).Objects.Count == 0 || GetGameState().GetLocationData(right).Objects.Count == 0 || GetGameState().GetLocationData(up).Objects.Count == 0 || GetGameState().GetLocationData(down).Objects.Count == 0)
                        {

                            if (rand.Next(0, 101) < 55 && numWallsRegular <= 70)
                            {
                                initLocation(LocationData.Tile.DESTRUCTIBLE_WALL, coords);
                                numWallsRegular++;
                            }
                        }
                    }
                }
            }*/

            for (int count = 0; count < numWallsRegular;)
            {
                // Generate random coordinates
                int randomX = rand.Next(1, this.width - 1);
                int randomY = rand.Next(1, this.height - 1);
                coords = Coords.coordsXY(randomX, randomY, this.width, this.height);
                
                // Get location data for coords
                LocationData loc = GetGameState().GetLocationData(coords);

                if (loc.Objects.Count == 0)
                {
                    Coords left = Coords.coordsXY(randomX - 1, randomY, width, height);
                    Coords right = Coords.coordsXY(randomX + 1, randomY, width, height);
                    Coords up = Coords.coordsXY(randomX, randomY - 1, width, height);
                    Coords down = Coords.coordsXY(randomX - 1, randomY + 1, width, height);
                    if (GetGameState().GetLocationData(left).Objects.Count == 0 || GetGameState().GetLocationData(right).Objects.Count == 0 || GetGameState().GetLocationData(up).Objects.Count == 0 || GetGameState().GetLocationData(down).Objects.Count == 0)
                    {
                        initLocation(LocationData.Tile.DESTRUCTIBLE_WALL, coords);
                        count++;
                    }
                }
            }
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

        void initLocation(LocationData.Tile tile, Coords coords)
        {
            List<LocationData.Tile> objects = new List<LocationData.Tile>();
            objects.Add(tile);
            LocationData datum = new LocationData(objects);
            this.gs.SetLocationData(datum, coords);

        }
    }
}
