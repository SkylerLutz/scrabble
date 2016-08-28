using System.Collections;
using System.Collections.Generic;
using System;

public sealed class Raycasting {

	public static List<Coordinate> horizontalHitTest(ScrabbleBoard board, Coordinate coordinate) {

		List<Coordinate> list = new List<Coordinate>();

		for (int i = coordinate.x; i < board.dimension; i++) {
			if (board.getTile(i, coordinate.y) != null) {
				list.Add(new Coordinate(i, coordinate.y));
			}
		}
		return list;
	}
	public static List<Coordinate> verticalHitTest(ScrabbleBoard board, Coordinate coordinate) {

		List<Coordinate> list = new List<Coordinate>();

		for (int j = coordinate.y; j < board.dimension; j++) {
			if (board.getTile(coordinate.x, j) != null) {
				list.Add(new Coordinate(coordinate.x, j));
			}
		}

		return list;
	}
	public static bool horizontalContains(List<Coordinate> coordinates, Coordinate coordinate, int length) {
		
		HashSet<int> fixedSet = new HashSet<int>();
		HashSet<int> other = new HashSet<int>();
		foreach (Coordinate c in coordinates) {
			fixedSet.Add(c.x);
		}
		for (int i = coordinate.x; i < coordinate.x + length; i++) {
			other.Add(i);
		}
		fixedSet.IntersectWith (other);
		return fixedSet.Count != 0;
	} 
	public static bool verticalContains(List<Coordinate> coordinates, Coordinate coordinate, int length) {
		
		HashSet<int> fixedSet = new HashSet<int>();
		HashSet<int> other = new HashSet<int>();
		foreach (Coordinate c in coordinates) {
			fixedSet.Add(c.y);
		}
		for (int i = coordinate.y; i < coordinate.y + length; i++) {
			other.Add(i);
		}
		fixedSet.IntersectWith(other);
		return fixedSet.Count != 0;
	} 
	public static ScrabbleScoringDirection getDirection(Coordinate[] coordinates) {

		if (coordinates.Length < 1) {
			return ScrabbleScoringDirection.HORIZONTAL;
		}
		Coordinate first = coordinates [0];


		if (coordinates.Length < 2) {
			return ScrabbleScoringDirection.HORIZONTAL;
		}
		Coordinate other = coordinates [1];


		if (first.x == other.x) {
			return ScrabbleScoringDirection.VERTICAL;
		} else if (first.y == other.y) {
			return ScrabbleScoringDirection.HORIZONTAL;
		}
		else {
			return ScrabbleScoringDirection.HORIZONTAL;
		}
	}
}
