using System.Collections;
using System.Collections.Generic;

public sealed class PlayerConfiguration {
	public int tileCount;
	public PlayerConfiguration(int tileCount) {
		this.tileCount = tileCount;
	}
}

public class Player {
	public List<Tile> tiles;
	public Player(List<Tile> tiles) {
		this.tiles = tiles;
	}
	public Player() {
		this.tiles = new List<Tile>();
	}
}