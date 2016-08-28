//using System.Collections;
//using System.Collections.Generic;

public sealed class PlayerMove: AbstractPlayerMove {

	//	List<Tile> tiles;
	//	List<Coordinate> coordinates;
	//
	public PlayerMove(Tile[] tiles, Coordinate[] coordinates) : base (tiles, coordinates) {

	}

	public override string ToString ()
	{
		return "tiles: " + stringify (tiles) + ", coordinates: " + stringify (coordinates);
	}

	private string stringify<T>(T[] array) {
		string s = "";
		foreach (T e in array) {
			s += e.ToString () + " ";
		}
		return s;
	}

}
