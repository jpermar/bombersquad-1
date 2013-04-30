using System;

namespace bombersquad_ai
{
	public class ConsoleCanvas
	{
		public ConsoleCanvas ()
		{
		}
		
		public void DrawGameState(GameState gs) {
			
			if (gs.GameOver) {
				Console.WriteLine("GAME OVER!");
				return;
			}
			int width = gs.Width;
			int height = gs.Height;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					LocationData loc = gs.GetLocationData(x, y);
					if (loc == null) {
						Console.WriteLine("location data null!!" + "(" + x + ", " + y + ")");
					}
					//LocationData.Tile tile = loc.LocationTile;
					char screenValue = GetLocationChar(loc);
					Console.Write(screenValue);
				}
				Console.WriteLine();
			}
			Console.WriteLine();
		}
		
		public char GetLocationCharOld(LocationData loc) {
			
			if (loc.Objects.Count == 0) {
				return '_';
			}
			
			LocationData.Tile[] objects = loc.Objects.ToArray();
			LocationData.Tile tile = objects[0];
			
			char tileChar = 'A';
			if (loc.Objects.Count > 1) {
				tileChar = '!';
			} else if (tile == LocationData.Tile.EXPLOSION) {
				tileChar = 'X';
			} else if (tile == LocationData.Tile.PLAYER) {
				tileChar = 'P';
			} else if (tile == LocationData.Tile.AI_1) {
				tileChar = '1';
			} else if (tile == LocationData.Tile.AI_2) {
				tileChar = '2';
			} else if (tile == LocationData.Tile.AI_3) {
				tileChar = '3';
			} else if (tile == LocationData.Tile.AI_4) {
				tileChar = '4';
			} else if (tile == LocationData.Tile.BOMB) {
				tileChar = 'B';
			} else if (tile == LocationData.Tile.DESTRUCTIBLE_WALL) {
				tileChar = '-';
			} else if (tile == LocationData.Tile.INDESTRUCTIBLE_WALL) {
				tileChar = '=';
			} 
			return tileChar;
		}


        public static char GetLocationChar(LocationData loc)
        {

            if (loc.Objects.Count == 0)
            {
                return '_';
            }

            LocationData.Tile[] objects = loc.Objects.ToArray();
            LocationData.Tile tile = objects[0];

            char tileChar = 'A';
            if (loc.Objects.Count > 1)
            {
                int numAI = 0;
                int numExplosions = 0;
                
                foreach(LocationData.Tile element in loc.Objects)
                {
                    if (element == LocationData.Tile.INDESTRUCTIBLE_WALL) { return '='; } // override
                    if (element == LocationData.Tile.BOMB) { return 'B'; } // override
                    if (element == LocationData.Tile.PLAYER) { return 'P'; } // override
                    if (element == LocationData.Tile.AI_1) { numAI++; }
                    if (element == LocationData.Tile.AI_2) { numAI++; }
                    if (element == LocationData.Tile.AI_3) { numAI++; }
                    if (element == LocationData.Tile.AI_4) { numAI++; }
                    if (element == LocationData.Tile.EXPLOSION) { numExplosions++; }
                }

                if (numExplosions > 1)
                {
                    tileChar = 'E';
                }
                else if (numAI > 1)
                {
                    switch (numAI)
                    {
                        case 2: tileChar = '@'; break;
                        case 3: tileChar = '#'; break;
                        case 4: tileChar = '$'; break;
                        default: tileChar = '!'; break;
                    }
                }
                else
                {
                    tileChar = '!';
                }
            }
            else if (tile == LocationData.Tile.EXPLOSION)
            {
                tileChar = 'e';
            }
            else if (tile == LocationData.Tile.PLAYER)
            {
                tileChar = 'P';
            }
            else if (tile == LocationData.Tile.AI_1)
            {
                tileChar = '1';
            }
            else if (tile == LocationData.Tile.AI_2)
            {
                tileChar = '2';
            }
            else if (tile == LocationData.Tile.AI_3)
            {
                tileChar = '3';
            }
            else if (tile == LocationData.Tile.AI_4)
            {
                tileChar = '4';
            }
            else if (tile == LocationData.Tile.BOMB)
            {
                tileChar = 'B';
            }
            else if (tile == LocationData.Tile.DESTRUCTIBLE_WALL)
            {
                tileChar = '-';
            }
            else if (tile == LocationData.Tile.INDESTRUCTIBLE_WALL)
            {
                tileChar = '=';
            }
            return tileChar;
        }


	}
}

