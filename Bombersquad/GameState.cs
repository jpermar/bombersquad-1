using System;
using System.Collections.Generic;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	public class GameState
	{

        public readonly int MAX_NUM_AI_BOMBERS = GameConfig.maxNumAIBombers; // config data
        public readonly int EXPLOSION_RADIUS = GameConfig.explosionRadius; // config data
        private readonly double EXPLOSION_TACTICAL_WEIGHT = 1;

		private LocationData[,] map;

		public LocationData[,] Map {
			get { return this.map; }
		}
		
		private bool gameOver;
		
		public bool GameOver {
			get { return gameOver; }
			set { gameOver = value; }
		}
		
		private int width;

		public int Width {
			get { return width; }
		}
		
		private int height;

		public int Height {
			get { return height; }
		}
		
		private PlayerState playerState;

		public PlayerState Player {
			get { return playerState; }
		}
		
		private PlayerState[] aiState;

		public PlayerState AI1State {
			get { return aiState [0]; }
		}
		
		public PlayerState AI2State {
			get { return aiState [1]; }
		}
		
		public PlayerState AI3State {
			get { return aiState [2]; }
		}
		
		public PlayerState AI4State {
			get { return aiState [3]; }
		}
		
		private int numAIBombermen;

		public int NumberAIBombermen {
			get { return this.numAIBombermen; }
		}
		
		private TimeSpan detonationTime;
		private Coords[] bombermanSpawnPoints;
		private List<Coords> liveBombs;
		
		public GameState (int width, int height, TimeSpan detonationTime)
		{
			
			this.width = width;
			this.height = height;
			
			this.map = new LocationData[width, height];
			for (int i = 0; i < width; i++) {
				for (int j = 0; j < height; j++) {
					List<LocationData.Tile> objects = new List<LocationData.Tile> ();
					this.map [i, j] = new LocationData (objects);
				}
			}
			
			this.gameOver = false;
			this.aiState = new PlayerState[MAX_NUM_AI_BOMBERS];
			this.numAIBombermen = 0;
			this.bombermanSpawnPoints = new Coords[MAX_NUM_AI_BOMBERS + 1];
			this.detonationTime = detonationTime;
			this.liveBombs = new List<Coords> ();
		}
		
		private bool IsInvalidLoc (int x, int y)
		{
			return ((x >= this.width) || (x < 0) || (y >= this.height) || (y < 0));
		}
		
		public LocationData GetLocationData (Coords coord)
		{
			int x = coord.getX ();
			int y = coord.getY ();
			return this.GetLocationData (x, y);
		}
		
		public LocationData GetLocationData (int x, int y)
		{
			int xLoc = x;
			if (xLoc >= this.width) {
				xLoc = this.width - 1;
			} else if (xLoc < 0) {
				xLoc = 0;
			}
			
			int yLoc = y;
			if (yLoc >= this.height) {
				yLoc = this.height - 1;
			} else if (yLoc < 0) {
				yLoc = 0;
			}
			return this.map [xLoc, yLoc];
		}
		
		public void SetLocationData (LocationData locationData, Coords coords)
		{
			int x = coords.getX ();
			int y = coords.getY ();
			this.map [x, y] = locationData;
		}
		
		public void UpdateGame (Action playerAction, Action[] botActions)
		{

            Console.WriteLine("UPDATE GAME");
			this.gameOver = true; //reset to false inside ClearExplosions()
			ClearExplosions ();
			ExplodeBombs ();
			RespawnPlayers ();
			
			PlayerState state = GetPlayerState (0);
			
			Coords coords = GetAgentCoords (0);
			
			int x = coords.getX ();
			int y = coords.getY ();
			
			if (playerAction != null) {
				bool legal = IsLegalAction (x, y, playerAction, state);
			
				
				if (legal) {
					ApplyAction (x, y, playerAction, state);
				}
			}
			
			for (int i = 0;  i < botActions.Length; i++) {
				if (botActions [i] != null) {
					Console.WriteLine ("getting player coords for bot " + i);
					coords = GetAgentCoords (i + 1);
			
					x = coords.getX ();
					y = coords.getY ();
				
					bool legal = IsLegalAction (x, y, botActions [i], GetPlayerState (i + 1));
					if (legal) {
						Console.WriteLine ("GameState applying, bot " + i + ", action " + botActions [i]);
						ApplyAction (x, y, botActions [i], GetPlayerState (i + 1));
					} else {
						Console.WriteLine ("Invalid move for bot " + i + ": " + botActions [i]);
					}
				}
			}
		}
		
		private void RespawnPlayers ()
		{
			if (this.playerState.Location == null) {
				this.playerState.Location = this.bombermanSpawnPoints [0];
				LocationData datum = GetLocationData (this.playerState.Location);
				datum.AddObject (this.playerState.PlayerObject);
			}
			
			for (int i = 0; i < MAX_NUM_AI_BOMBERS; i++) {
				PlayerState state = this.aiState [i];
				if (state != null) {
					if (state.Location == null) {
						state.Location = this.bombermanSpawnPoints [i + 1];
						LocationData datum = GetLocationData (state.Location);
						datum.AddObject (state.PlayerObject);
					}
				}
			}
		}
		
		public Coords GetAgentCoords (int playerNum)
		{
			PlayerState playerState = GetPlayerState (playerNum);
			if (playerState == null) {
				return null;
			}
			return playerState.Location;
		}
		
		private void ClearExplosions ()
		{
			int width = this.width;
			int height = this.height;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Coords coord = Coords.coordsXY (x, y, width, height);
					LocationData loc = GetLocationData (coord);
					List<LocationData.Tile> objects = loc.Objects;
					for (int i = objects.Count - 1; i >= 0; i--) {
						//foreach (LocationData.Tile obj in objects) {
						LocationData.Tile obj = objects [i];
						if (obj == LocationData.Tile.EXPLOSION) {
							loc.RemoveObject (obj);
						}
						
						if (obj == LocationData.Tile.DESTRUCTIBLE_WALL) {
							this.gameOver = false;
						}
					}
				}
			}
		}
		
		//kill bombermen, chain-reaction bombs, reset bomb counts, respawn killed bombermen...
		private void ExplodeBombs ()
		{
			int width = this.width;
			int height = this.height;
			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					Coords coord = Coords.coordsXY (x, y, width, height);
					LocationData loc = GetLocationData (x, y);
					List<LocationData.Tile> objects = loc.Objects;
					for (int i = objects.Count-1; i >= 0; i--) {
						//foreach (LocationData.Tile obj in objects) {
						//can blow up multiple bombs... check for exception
						try {
							LocationData.Tile obj = objects [i];
							if (obj == LocationData.Tile.BOMB) {
								DateTime dropTime = loc.BombDropTime;
								DateTime now = DateTime.Now;
								TimeSpan gap = now - dropTime;
								if (gap >= this.detonationTime) {
									explodeBomb (coord);
								}
							}
						} catch (ArgumentOutOfRangeException e) {
							continue;
						}
						
					}
				}
			}
			
		}
		
		//chain reaction... explodes bombs encountered as well
		private void explodeBomb (Coords bombCoord)
		{
			//remove bomb
			//explode in manhattan radius, killing everything in path and destroying first wall encountered
			LocationData datum = GetLocationData (bombCoord);
			LocationData.Tile bomb = LocationData.Tile.BOMB;
			datum.RemoveObject (bomb);
			this.liveBombs.Remove (bombCoord);
			
			PlayerState bombOwner = datum.BombOwner;
			bombOwner.Bombs = bombOwner.Bombs + 1;
			
			explosionUpdate (bombCoord);
			
			//chain reaction...
			
			int radius = datum.ExplosionRadius;
			
			int x = bombCoord.getX ();
			int y = bombCoord.getY ();
			//search every cardinal direction... blow up any player encountered and first destructible wall encountered
			//west
			for (int i = 1; i <= radius; i++) {
				try {
					Coords next = Coords.coordsXY (x - i, y, width, height);
					bool halt = explosionUpdate (next);
					if (halt) {
						break;
					}
				} catch (ArgumentException e) {
					break;
				}
			}

			//north
			for (int i = 1; i <= radius; i++) {
				try {
					Coords next = Coords.coordsXY (x, y - i, width, height);
					bool halt = explosionUpdate (next);
					if (halt) {
						break;
					}
				} catch (ArgumentException e) {
					break;
				}
			}

			//south
			for (int i = 1; i <= radius; i++) {
				try {
					Coords next = Coords.coordsXY (x, y + i, width, height);
					bool halt = explosionUpdate (next);
					if (halt) {
						break;
					}
				} catch (ArgumentException e) {
					break;
				}
			}

			//east
			for (int i = 1; i <= radius; i++) {
				try {
					Coords next = Coords.coordsXY (x + i, y, width, height);
					bool halt = explosionUpdate (next);
					if (halt) {
						break;
					}
				} catch (ArgumentException e) {
					break;
				}
			}

		}
		
		//update LocationData fields at coord
		private bool explosionUpdate (Coords coord)
		{
			bool haltExplosion = false;
			
			LocationData locationData = GetLocationData (coord);
			locationData.AddObject (LocationData.Tile.EXPLOSION);
			if (locationData.HasDestructibleWall ()) {// == LocationData.Tile.DESTRUCTIBLE_WALL) {
				detonateWall (coord);
				haltExplosion = true; //explosion finished in this direction
			}
			
			if (locationData.HasBomberman ()) {
				//kill them all!
				KillBombermen (coord);
			}
			
			if (locationData.HasBomb ()) { // == LocationData.Tile.BOMB) {
				explodeBomb (coord);
			}
			
			if (locationData.HasIndestructibleWall ()) { // == LocationData.Tile.INDESTRUCTIBLE_WALL) {
				//stop explosion...
				haltExplosion = true;
			}
			return haltExplosion;
		}
		
		private void detonateWall (Coords coord)
		{
			LocationData datum = GetLocationData (coord);
			datum.RemoveObject (LocationData.Tile.DESTRUCTIBLE_WALL);
		}
		
		private void KillBombermen (Coords coord)
		{
			LocationData datum = GetLocationData (coord);
			List<LocationData.Tile> objects = datum.Objects;
			for (int i = objects.Count-1; i >= 0; i--) {
				//foreach (LocationData.Tile obj in objects) {
				//if it's a bomberman, remove it...
				LocationData.Tile obj = objects [i];
				if (datum.isBombermanTile (obj)) {
					objects.RemoveAt (i);
					PlayerState state = GetPlayerState (obj);
					state.Location = null;
				}
				
			}
		}
		
		private void ApplyAction (int x, int y, Action action, PlayerState state)
		{
			
			//Console.WriteLine (action.ActionName.ToString ());
			
			if (!action.ActionName.Equals (Action.ActionType.BOMB)) {
				int xCoord = x;
				int yCoord = y;
				if (action.ActionName.Equals (Action.ActionType.NORTH)) {
					yCoord = y - 1;
				} else if (action.ActionName.Equals (Action.ActionType.SOUTH)) {
					yCoord = y + 1;
					//Console.WriteLine ("moving south!!");
				} else if (action.ActionName.Equals (Action.ActionType.EAST)) {
					//Console.WriteLine ("moving east!!");
					xCoord = x + 1;
				} else if (action.ActionName.Equals (Action.ActionType.WEST)) {
					//Console.WriteLine ("moving west!!");
					xCoord = x - 1;
				}
					
				LocationData datum = GetLocationData (x, y);
				LocationData.Tile newObject = state.PlayerObject;
				datum.RemoveObject (newObject);
				LocationData target = GetLocationData (xCoord, yCoord);
				target.AddObject (newObject);
				state.Location = Coords.coordsXY (xCoord, yCoord, this.width, this.height);
			} else {
				//check that agent has enough bombs...
				//if (state.Bombs > 0) {
					LocationData datum = GetLocationData (x, y);
					datum.BombDropTime = DateTime.Now;
					datum.ExplosionRadius = EXPLOSION_RADIUS;
					datum.BombOwner = state;
					datum.AddObject (LocationData.Tile.BOMB);
					state.Bombs = state.Bombs - 1;
					this.liveBombs.Add (Coords.coordsXY (x, y, width, height));
				//}
			}
		}
		
		/*
		private LocationData.Tile GetPlayerTile(int playerNum) {
			if (playerNum == 0) {
				return LocationData.Tile.PLAYER;
			} else if (playerNum == 1) {
				return LocationData.Tile.AI_1;
			} else if (playerNum == 2) {
				return LocationData.Tile.AI_2;
			}  else if (playerNum == 3) {
				return LocationData.Tile.AI_3;
			} else {
				return LocationData.Tile.AI_4;
			}
		}
		*/
		
		private bool IsLegalAction (int x, int y, Action action, PlayerState state)
		{
			//if a move in a direction, check that the tile in the direction is empty
			
			//Console.WriteLine ("checking action " + action.ActionName.ToString ());
			if (!action.ActionName.Equals (Action.ActionType.BOMB)) {
				int xCoord = x;
				int yCoord = y;
				if (action.ActionName.Equals (Action.ActionType.NORTH)) {
					yCoord = y - 1;
				} else if (action.ActionName.Equals (Action.ActionType.SOUTH)) {
					//Console.WriteLine ("found south");
					yCoord = y + 1;
				} else if (action.ActionName.Equals (Action.ActionType.EAST)) {
					xCoord = x + 1;
				} else if (action.ActionName.Equals (Action.ActionType.WEST)) {
					xCoord = x - 1;
				}
					
				if (IsInvalidLoc (xCoord, yCoord)) {
					//Console.WriteLine ("invalid");
					return false;
				} else {
				
					LocationData data = GetLocationData (xCoord, yCoord);
				
					List<LocationData.Tile> objects = data.Objects;
					
					bool valid = true;
					//Console.WriteLine ("checking empty");
					foreach (LocationData.Tile datum in objects) {
						//Console.WriteLine ("datum" + datum.ToString ());
						if ((datum.Equals (LocationData.Tile.DESTRUCTIBLE_WALL)) ||
						(datum.Equals (LocationData.Tile.INDESTRUCTIBLE_WALL)) || (datum.Equals(LocationData.Tile.BOMB))) {
							valid = false;
							//Console.WriteLine ("not empty!");
						}
                        if (state != null)
                        {
                            // Player cannot walk through AI
                            if (state.PlayerObject.Equals(LocationData.Tile.PLAYER))
                            {
                                if (datum.Equals(LocationData.Tile.AI_1)
                                    || datum.Equals(LocationData.Tile.AI_2)
                                    || datum.Equals(LocationData.Tile.AI_3)
                                    || datum.Equals(LocationData.Tile.AI_4))
                                {
                                    valid = false;
                                }
                            }
                            // AI cannot walk through player
                            if (state.PlayerObject.Equals(LocationData.Tile.AI_1)
                                    || state.PlayerObject.Equals(LocationData.Tile.AI_2)
                                    || state.PlayerObject.Equals(LocationData.Tile.AI_3)
                                    || state.PlayerObject.Equals(LocationData.Tile.AI_4))
                            {
                                if (datum.Equals(LocationData.Tile.PLAYER))
                                {
                                    valid = false;
                                }
                            }
                        }
					}
					
					//Console.WriteLine ("return valid " + valid);
					return valid;
				}
			} else {
				//if a bomb, check that the player has bombs available to place
                //and that there is no bomb already at the location
                Coords coord = Coords.coordsXY(x, y, width, height);
                LocationData datum = GetLocationData(coord);

                if ((!datum.HasBomb()) && (state.Bombs > 0)) {
					return true;
				} else {
					return false;
				}
					
			}
			
		}
		
		private PlayerState GetPlayerState (LocationData.Tile tile)
		{
			if (tile == LocationData.Tile.PLAYER) {
				return this.playerState;
			} else if (tile == LocationData.Tile.AI_1) {
				return this.AI1State;
			} else if (tile == LocationData.Tile.AI_2) {
				return this.AI2State;
			} else if (tile == LocationData.Tile.AI_3) {
				return this.AI3State;
			} else if (tile == LocationData.Tile.AI_4) {
				return this.AI4State;
			}
			
			return null;
		}
		
		public PlayerState GetPlayerState (int player)
		{
			if (player == 0) {
				return this.playerState;
			} else {
				return this.aiState [player - 1];
			}
		}
		
		//Pass in LocationData.Tile.PLAYER or LocationData.Tile.AI_1, etc. 
		public void InitBomberman (LocationData.Tile tile, Coords coords)
		{
			PlayerState state = new PlayerState (tile);
			state.Bombs = 1;
			state.Location = coords;
			if (tile.Equals (LocationData.Tile.PLAYER)) {
                //state.Bombs = 4;
                this.playerState = state;
				this.bombermanSpawnPoints [0] = coords;
			} else if (tile.Equals (LocationData.Tile.AI_1)) {
				this.aiState [0] = state;
				this.bombermanSpawnPoints [1] = coords;
				this.numAIBombermen++;
			} else if (tile.Equals (LocationData.Tile.AI_2)) {
				this.aiState [1] = state;
				this.bombermanSpawnPoints [2] = coords;
				this.numAIBombermen++;
			} else if (tile.Equals (LocationData.Tile.AI_3)) {
				this.aiState [2] = state;
				this.bombermanSpawnPoints [3] = coords;
				this.numAIBombermen++;
			} else if (tile.Equals (LocationData.Tile.AI_4)) {
				this.aiState [3] = state;
				this.bombermanSpawnPoints [4] = coords;
				this.numAIBombermen++;
			}
		}
		
		public List<Coords> GetAdjacentAccessibleTiles (int tile,
			bool isBombPassable, bool isDestructiblePassable)
		{
			Coords coords = Coords.coordsTileNum (this.width, this.height,
				tile);
			
			/*
			bool match = false;
			if ((coords.getX() == 18) && (coords.getY() == 18)) {
				match = true;
			}
			*/
			List<Coords> possibleCoords = new List<Coords> ();

			try {
				Coords coordsNorth = Coords.coordsXY (coords.getX (),
					coords.getY () - 1, this.width, this.height);
				Action action = new Action (Action.ActionType.NORTH);
				LocationData datum = GetLocationData (coordsNorth);
				if (isDestructiblePassable) {
					if (datum.HasDestructibleWall () || IsLegalAction (coords.getX (), coords.getY (), action, null)) {
						possibleCoords.Add (coordsNorth);
					}
				} else {
					bool legal = IsLegalAction (coords.getX (), coords.getY (), action, null);
					if (legal) {
						if (!isBombPassable) {
							if (!datum.HasBomb()) {
								possibleCoords.Add (coordsNorth);
							}
						} else {
							possibleCoords.Add (coordsNorth);
						}
					}
				}
			} catch (ArgumentException e) {
				// ignore... could be trying a invalid tile (outside the map)
			}

			try {
				Coords coordsSouth = Coords.coordsXY (coords.getX (),
					coords.getY () + 1, this.width, this.height);
				Action action = new Action (Action.ActionType.SOUTH);
				LocationData datum = GetLocationData (coordsSouth);
				if (isDestructiblePassable) {
					if (datum.HasDestructibleWall () || IsLegalAction (coords.getX (), coords.getY (), action, null)) {
						possibleCoords.Add (coordsSouth);
					}
				} else {
					bool legal = IsLegalAction (coords.getX (), coords.getY (), action, null);
					if (legal) {			
						if (!isBombPassable) {
							if (!datum.HasBomb()) {
								possibleCoords.Add (coordsSouth);
							}
						} else {
							possibleCoords.Add (coordsSouth);
						}
					}
				}
			} catch (ArgumentException e) {
				// ignore... could be trying a invalid tile (outside the map)
			}

			try {
				Coords coordsEast = Coords.coordsXY (coords.getX () + 1,
					coords.getY (), this.width, this.height);
				Action action = new Action (Action.ActionType.EAST);
				LocationData datum = GetLocationData (coordsEast);
				if (isDestructiblePassable) {
					if (datum.HasDestructibleWall () || IsLegalAction (coords.getX (), coords.getY (), action, null)) {
						possibleCoords.Add (coordsEast);
					}
				} else {
					bool legal = IsLegalAction (coords.getX (), coords.getY (), action, null);
					if (legal) {
						if (!isBombPassable) {
							if (!datum.HasBomb()) {
								possibleCoords.Add (coordsEast);
							}
						} else {
							possibleCoords.Add (coordsEast);
						}
					}
				}
			} catch (ArgumentException e) {
				// ignore... could be trying a invalid tile (outside the map)
			}

			try {
				Coords coordsWest = Coords.coordsXY (coords.getX () - 1,
					coords.getY (), this.width, this.height);
				Action action = new Action (Action.ActionType.WEST);
				LocationData datum = GetLocationData (coordsWest);
				if (isDestructiblePassable) {
					if (datum.HasDestructibleWall () || IsLegalAction (coords.getX (), coords.getY (), action, null)) {
						possibleCoords.Add (coordsWest);
					}
				} else {
					bool legal = IsLegalAction (coords.getX (), coords.getY (), action, null);
					if (legal) {
						if (!isBombPassable) {
							if (!datum.HasBomb()) {
								possibleCoords.Add (coordsWest);
							}
						} else {
							possibleCoords.Add (coordsWest);
						}
					
					}
				}
			} catch (ArgumentException e) {
				// ignore... could be trying a invalid tile (outside the map)
			}
			
			/*
			Console.Write("GAMESTATE Coords adjacent to " + coords + ": ");
			foreach (Coords coord in possibleCoords) {
					Console.Write (coord);
			}
			Console.WriteLine();
			*/
			
			return possibleCoords;
		}
		
		public bool isOnExplosionPath (Coords coord)
		{
			//check all live bombs and bomb radius
			//if coord is on the explosion path, return true
			//otherwise false
			foreach (Coords bomb in this.liveBombs) {
				LocationData datum = GetLocationData (bomb);
				int radius = datum.ExplosionRadius;
				bool test = (coord.Equals(bomb) || coord.isCardinalEastOf (bomb, radius) || coord.isCardinalNorthOf (bomb, radius) || coord.isCardinalSouthOf (bomb, radius) || coord.isCardinalWestOf (bomb, radius));
				if (test) {
					return true;
				}
			}
			
			return false;
		}
        public Coords getBombOnExplosionPath(Coords coord)
        {
            //check all live bombs and bomb radius
            //if coord is on the explosion path, return the bomb that's going to explode
            //otherwise return null
            foreach (Coords bomb in this.liveBombs)
            {
                LocationData datum = GetLocationData(bomb);
                int radius = datum.ExplosionRadius;
                bool test = (coord.Equals(bomb) || coord.isCardinalEastOf(bomb, radius) || coord.isCardinalNorthOf(bomb, radius) || coord.isCardinalSouthOf(bomb, radius) || coord.isCardinalWestOf(bomb, radius));
                if (test)
                {
                    return bomb;
                }
            }

            return null;
        }

        //this is called by PathPlan when creating connections for the A* graph
        //That method adds this value to the base PathPlan.MOVEMENT_COST
        //this calculations uses a weight value and a tactical value
        //weight needs to be tuned so that bombermen avoid dangerous tiles, but do not ignore
        //them altogether
        public double tacticalCost(Coords coord)
        {
            Coords bomb = getBombOnExplosionPath(coord);
            if (bomb != null)
            {
                double tacticalWeight = EXPLOSION_TACTICAL_WEIGHT;
                //linear function normalized to [0.1..1.0] based on time to explode
                LocationData datum = GetLocationData(bomb);
                DateTime dropTime = datum.BombDropTime;
                DateTime now = DateTime.Now;
                TimeSpan gap = now - dropTime;

                //this.detonationTime is the maximum time to detonation
                //normalize gap between [0, this.detonationTime]
                double minBombCost = 0.1;
                double maxBombCost = 1.0;

                //ensure gap is in range... shouldn't be necessary but just in case..
                if (gap.Ticks < 0) { gap = new TimeSpan(0, 0, 0); }
                if (gap > this.detonationTime) { gap = this.detonationTime; }
                double gapPercent = gap.Ticks / this.detonationTime.Ticks;

                //normalize gapPercent to bomb cost range
                double tacticalValue = 0.1 + gapPercent * (maxBombCost - minBombCost);

                double tacticalCost = tacticalWeight * tacticalValue;
                return tacticalCost;
            }
            else
            {
                return 0;
            }
        }
    }
}

