
//using System.Collections;
//using System.Collections.Generic;

public abstract class AbstractPlayerMove {	
	public Tile[] tiles;
	public Coordinate[] coordinates;

	public AbstractPlayerMove(Tile[] tiles, Coordinate[] coordinates) {
		this.tiles = tiles;
		this.coordinates = coordinates;
	}
}  