using System;

namespace bombersquad_ai
{
	namespace astar
	{
		public class Coords
		{
			private int x;
			private int y;
			private int mapWidth;
			public int Width {
				get { return this.mapWidth; }
			}
			private int mapHeight;
			public int Height {
				get { return this.mapHeight; }
			}

			protected Coords (int x, int y, int mapWidth, int mapHeight)
			{
				this.mapWidth = mapWidth;
				this.mapHeight = mapHeight;
				this.x = x;
				this.y = y;
			}

			public static Coords coordsTileNum (int width, int height, int tileNum)
			{
				int y = (int)Math.Floor ((double)(tileNum / width));
				int x = tileNum % width;
				Coords coords = new Coords (x, y, width, height);
				if (!(coords.isValid ())) {
					String msg = coords.ToString () + " invalid due to max width of " + width + " and max height of " + height;
					throw new ArgumentException(msg);
				}
				return coords;
			}

			public static Coords coordsXY (int x, int y, int width, int height)
			{
				Coords coords = new Coords (x, y, width, height);
				if (!(coords.isValid ())) {
					String msg = coords.ToString () + " invalid due to max width of " + width + " and max height of " + height;
					throw new ArgumentException (msg);
				}
				return coords;
			}

			public int getTileNum ()
			{
				int tileNum = this.y * this.mapWidth + this.x;
				return tileNum;
			}

			public int getX ()
			{
				return this.x;
			}

			public int getY ()
			{
				return this.y;
			}
	
			/**
	 * is this a valid (x,y) coordinate? (i.e., actually on the map?)
	 * @return true if valid, false otherwise
	 */
			public bool isValid ()
			{
				return ((this.x >= 0) && (this.x < this.mapWidth) && (this.y >= 0) && (this.y < this.mapHeight));
			}
			
			public bool isNorthOf(Coords coord) {
				return (this.y < coord.y);
			}

			public bool isWestOf(Coords coord) {
				return (this.x < coord.x);
			}

			public bool isEastOf(Coords coord) {
				return (this.x > coord.x);
			}

			public bool isSouthOf(Coords coord) {
				return (this.y > coord.y);
			}

			public bool isCardinalNorthOf(Coords coord, int dist) {
				int minY = coord.y - dist;
				if (minY < 0) { minY = 0; }
				return ((this.x == coord.x) && (this.y < coord.y) && (this.y >= minY));
			}

			public bool isCardinalWestOf(Coords coord, int dist) {
				int minX = coord.x - dist;
				if (minX < 0) { minX = 0; }
				return ((this.y == coord.y) && (this.x < coord.x) && (this.x >= minX));
			}

			public bool isCardinalEastOf(Coords coord, int dist) {
				int maxX = coord.x + dist;
				if (maxX > mapWidth - 1) { maxX = mapWidth - 1; }
				return ((this.y == coord.y) && (this.x > coord.x) && (this.x <= maxX));
			}

			public bool isCardinalSouthOf(Coords coord, int dist) {
				int maxY = coord.y + dist;
				if (maxY > mapHeight - 1) { maxY = mapHeight - 1; }
				return ((this.x == coord.x) && (this.y > coord.y) && (this.y <= maxY));
			}

			public override string ToString ()
			{
				return "(" + this.x + "," + this.y + ")";
			}

			public override int GetHashCode ()
			{
				return this.y * this.mapWidth + this.x;
			}

			public override bool Equals (Object obj)
			{
				bool result = (obj is Coords);
				if (!result) {
					return false;
				}

				Coords other = (Coords)obj;
				return ((this.x == other.x) && (this.y == other.y));
			}
		}
	}
}

