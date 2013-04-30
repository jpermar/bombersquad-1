using System;
using System.Collections.Generic;

namespace bombersquad_ai
{
	public class LocationData
	{
		
		private DateTime bombDropTime;
		public DateTime BombDropTime {
			get { return this.bombDropTime; }
			set { this.bombDropTime = value; }
		}
		
		private int explosionRadius;
		public int ExplosionRadius {
			get { return this.explosionRadius; }
			set { this.explosionRadius = value; }
		}
		
		private PlayerState bombOwner;
		public PlayerState BombOwner {
			get { return this.bombOwner; }
			set { this.bombOwner = value; }
		}
		
		//public enum Tile { PLAYER, AI_1, AI_2, AI_3, AI_4, INDESTRUCTIBLE_WALL, DESTRUCTIBLE_WALL, EMPTY, BOMB };
		public enum Tile { PLAYER, AI_1, AI_2, AI_3, AI_4, INDESTRUCTIBLE_WALL, DESTRUCTIBLE_WALL, BOMB, EXPLOSION };
		
		private List<Tile> objects;
		public List<Tile> Objects {
			get { return objects; }
		}
		
		public LocationData (List<Tile> objects)
		{
			if (objects == null) {
				throw new System.ArgumentException("Parameter cannot be null", "objects");
			}
			this.objects = objects;
		}
		
		
		public void AddObject(Tile datum) {
			this.objects.Add(datum);
		}
		
		public void RemoveObject(Tile datum) {
			this.objects.Remove(datum);
		}
		
		public bool HasBomberman() {
			foreach(Tile obj in objects) {
				if (isBombermanTile(obj)) {
					return true;
				}
			}
			return false;
		}
		
		public bool HasDestructibleWall() {
			foreach(Tile obj in objects) {
				if (obj == Tile.DESTRUCTIBLE_WALL) {
					return true;
				}
			}
			return false;
		}
		
		public bool HasBomb() {
			foreach(Tile obj in objects) {
				if (obj == Tile.BOMB) {
					return true;
				}
			}
			return false;
		}
		
		public bool HasIndestructibleWall() {
			foreach(Tile obj in objects) {
				if (obj == Tile.INDESTRUCTIBLE_WALL) {
					return true;
				}
			}
			return false;
		}
		
		public bool isBombermanTile(LocationData.Tile obj) {
			return ((obj == Tile.AI_1) || (obj == Tile.AI_2) || (obj == Tile.AI_3) || (obj == Tile.AI_4) || (obj == Tile.PLAYER));
		}
		
		public override string ToString ()
		{
			return string.Format ("[LocationData: Objects={0}]", Objects);
		}
		
	}
}

