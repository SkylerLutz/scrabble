using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public sealed class ScrabblePlayerMoveScoring {

	private ScrabbleWordScoring scoring;
	private Trie dict;
	public ScrabblePlayerMoveScoring(ScrabbleWordScoring scoring, Trie dict) {
		this.scoring = scoring;
		this.dict = dict;
	}	

	public int score(ScrabbleBoard board, Coordinate[] placedTileCoordinates, ScrabbleScoringDirection direction) {
		
//		Debug.Log("Player placed: " + placedTileCoordinates.ToArray().ToString() + " (" + direction + ").");
//		foreach (Coordinate c in placedTileCoordinates) {
//			Debug.Log (c);
//		}
		if (placedTileCoordinates.Length == 0) {
//			Debug.Log ("nothing was placed");
			return -1; // nothing was placed
		}
		Coordinate first = placedTileCoordinates[0];
		List<TileRange> ranges = new List<TileRange>();
//		Debug.Log ("first: " + first);
		TileRange root = yield(board, first, direction);
//		Debug.Log("root... " + root);
		String word = read(board, root);
		if (!dict.isWord(word)) {
//			Debug.Log (word + " is not a word (root)");
			return -1;
		}
		ranges.Add(root);

		ScrabbleScoringDirection orthogonal = ScrabbleScoringDirectionHelper.orthogonal(direction);
		foreach (Coordinate coordinate in placedTileCoordinates) {
			TileRange range = yield(board, coordinate, orthogonal);
			if (range.length > 1) {
				String crossword = read(board, range);
				if (!dict.isWord(crossword)) {
//					Debug.Log (crossword + " is not a word (crossword)");
					return -1;
				}
				ranges.Add(range);
			}
		}

		int score = 0;
		foreach (TileRange range in ranges) {
			//System.out.println("scoring range... " + range);
			int wordScore = scoring.scoreWord(board, range.coordinate.x, range.coordinate.y, range.length, range.direction);
			//System.out.println("Range " + range + " scored: " + wordScore);
			if (wordScore == -1) {
				//System.out.println("Failing fast on range: " + range);
				return -1;
			}	
			score += wordScore;
		}
		return score;
	}

	private TileRange yield(ScrabbleBoard board, Coordinate coordinate, ScrabbleScoringDirection direction) {
		int x = coordinate.x;
		int y = coordinate.y;
		Tile tile = board.getTile(x, y);
		Tile prev = board.getTile(x, y);
		// back up to the beginning of the word
		while (tile != null) {
			prev = tile;
			x -= ScrabbleScoringDirectionHelper.horizontalDelta(direction);
			y -= ScrabbleScoringDirectionHelper.verticalDelta(direction);
			tile = board.getTile(x, y);
		}
		Tile origin = prev;
//		Debug.Log ("origin: " + origin);
		int length = 0;
		Tile curr = origin;
		// move back into position of origin
		x += ScrabbleScoringDirectionHelper.horizontalDelta(direction); 
		y += ScrabbleScoringDirectionHelper.verticalDelta(direction);
		int origin_x = x;
		int origin_y = y;
		while (curr != null) {
			x += ScrabbleScoringDirectionHelper.horizontalDelta(direction);
			y += ScrabbleScoringDirectionHelper.verticalDelta(direction);
			length += 1;
			curr = board.getTile(x, y);
		}
		return new TileRange(new Coordinate(origin_x, origin_y), length, direction);
	}
	
	private String read(ScrabbleBoard board, TileRange range) {
		String word = "";
		int x = range.coordinate.x, y = range.coordinate.y;
		for (int i = 0; i < range.length; i++) {
			char letter = board.getTile(x, y).getLetter();
			word += letter;
			x += ScrabbleScoringDirectionHelper.horizontalDelta(range.direction);
			y += ScrabbleScoringDirectionHelper.verticalDelta(range.direction);
		}
		return word;
	}
}
