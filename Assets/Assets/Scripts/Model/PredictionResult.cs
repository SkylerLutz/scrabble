//using System.Collections;
//using System.Collections.Generic;
using System;


public sealed class PredictionResult: AbstractPlayerMove, IComparable<PredictionResult> {
	public int score;
	public String rootWord;
	public PredictionResult(Tile[] tiles, Coordinate[] coordinates, int score, String rootWord) : base (tiles, coordinates) {
		this.score = score;
		this.rootWord = rootWord;
	}	
	public int CompareTo(PredictionResult other) {
		return -this.score.CompareTo(other.score);
	}
	public override String ToString() {
		return rootWord + " (" + score + " points)";
	}
}
