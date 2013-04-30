using System;
using bombersquad_ai.astar;

namespace bombersquad_ai
{
	public class PlayerState
	{
		private int bombs;
		public int Bombs {
			get { return bombs; }
			set { bombs = value; }
		}
		
		private Coords location; // null indicates dead
		public Coords Location {
			get { return this.location; }
			set { this.location = value; }
		}

		private LocationData.Tile playerObject;
		public LocationData.Tile PlayerObject {
			get { return this.playerObject; }
		}
		
		public PlayerState (LocationData.Tile playerObject)
		{
			this.playerObject = playerObject;
		}
	}
}

