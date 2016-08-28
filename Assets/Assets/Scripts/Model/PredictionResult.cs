using System.Collections;
using System.Collections.Generic;
using System;


public sealed class PredictionResult: AbstractPlayerMove, IComparable<PredictionResult> {
	public int score;

	public PredictionResult(List<Tile> tiles, List<Coordinate> coordinates, int score) : base (tiles, coordinates) {
		this.score = score;
	}	
	public int CompareTo(PredictionResult other) {
		return this.score.CompareTo(other.score);
	}
}
