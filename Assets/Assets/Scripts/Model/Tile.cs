using System.Collections;
using System;

public enum TileType{
	LETTER,
	WILDCARD
}
public sealed class Tile: IEquatable<Tile> { 

	public TileType type;
	private char letter;

	public Tile(TileType type, char letter) {
		this.type = type;
		this.letter = letter;
	}       

	public char getLetter() {
		return this.letter;
		//		switch(this.type) {
		//		case TileType.LETTER: return this.letter;
		//		case TileType.WILDCARD: return null;
		//		}
		//		return null; 
	}       
	public bool Equals(Tile other) { 
		return (this.letter == other.letter);
	}       
	public override string ToString() { 
		switch(this.type) {
		case TileType.LETTER: return "" + this.letter;
		}
		return null;
	}       
}      

public class TileRange {

	public Coordinate coordinate;
	public int length;
	public ScrabbleScoringDirection direction;
	public TileRange(Coordinate coordinate, int length, ScrabbleScoringDirection direction) {
		this.coordinate = coordinate;
		this.length = length;
		this.direction = direction;
	}
	public override string ToString() {
		return direction + " word from " + coordinate + ", " + length + " letters.";
	}
}
