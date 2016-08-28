
using System.Collections;
using System.Collections.Generic;

public abstract class AbstractPlayerMove {	
	public List<Tile> tiles;
	public List<Coordinate> coordinates;

	public AbstractPlayerMove(List<Tile> tiles, List<Coordinate> coordinates) {
		this.tiles = tiles;
		this.coordinates = coordinates;
	}
}  