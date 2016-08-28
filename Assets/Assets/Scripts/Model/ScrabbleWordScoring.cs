
using System.Collections;
using System.Collections.Generic;
using System;

public sealed class ScrabbleWordScoring {

	private ScrabbleScoringPolicy policy;

	public ScrabbleWordScoring(ScrabbleScoringPolicy policy) {
		this.policy = policy;
	}	

	public int scoreWord(ScrabbleBoard board, int i, int j, int length, ScrabbleScoringDirection direction) {

		
		//System.out.println("origin: " + new Coordinate(i, j) + ".");
		String buffer = "";
		List<TileSpace> spaces = new List<TileSpace>();

		for (int x = 0; x < length; x++) {
			Tile tile = board.getTile(i, j);
			//System.out.println("Picked up tile " + tile + " at " + new Coordinate(i, j) + ".");
			if (tile == null) {
				// TODO: Throw a proper exception.
//				System.out.println("Out of bounds");
			}	
			buffer = buffer + tile.getLetter();

			TileSpace space = board.getSpace(i, j);
			if (space == null) {
				// TODO: Throw a proper exception.
//				System.out.println("Out of bounds");
			}	
			spaces.Add(space);
			i += ScrabbleScoringDirectionHelper.horizontalDelta(direction);
			j += ScrabbleScoringDirectionHelper.verticalDelta(direction);
		}
		return policy.score(buffer, spaces);
	}
}
