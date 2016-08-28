using System.Collections;
using System;

public class Coordinate: IComparable<Coordinate> {
	public int x;
	public int y;
	public Coordinate(int x, int y) {
		this.x = x;
		this.y = y;
	}
	public override string ToString() {
		return "(" + x + ", " + y + ")";
	}
	public int CompareTo(Coordinate other) {
		return (this.x == other.x && this.y == other.y) ? 0 : 1;
	}
}