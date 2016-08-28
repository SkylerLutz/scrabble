using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public sealed class PredictionTests {
	
	public static void Main() {
//		microTest(new Coordinate(2,2));
		standardTest();
//		hitTest();
	}

	public static void hitTest() {

		ScrabbleScoringPolicy policy = new ScrabbleScoringPolicy();

		ScrabbleWordScoring wordScoring = new ScrabbleWordScoring(policy);

		Trie dict = new Trie();
		ScrabblePlayerMoveScoring scoring = new ScrabblePlayerMoveScoring(wordScoring, dict);

		ScrabbleBoard board = new ScrabbleBoard(ScrabbleBoardConfiguration.STANDARD);
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'H'), 3, 0);
		board.setTile(new Tile(TileType.LETTER, 'E'), 3, 1);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 2);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 3);
		board.setTile(new Tile(TileType.LETTER, 'O'), 3, 4);

		board.setTile(new Tile(TileType.LETTER, 'Y'), 0, 2);
		board.setTile(new Tile(TileType.LETTER, 'I'), 1, 2);
		board.setTile(new Tile(TileType.LETTER, 'E'), 2, 2);
		board.setTile(new Tile(TileType.LETTER, 'D'), 4, 2);

		// _ | _ | _ | H | _ | _ | _ |
		// _ | _ | _ | E | _ | _ | _ |
		// Y | I | E | L | D | _ | _ |
		// _ | _ | _ | L | _ | _ | _ |
		// _ | _ | _ | O | _ | _ | _ |
		// _ | _ | _ | _ | _ | _ | _ |
		// _ | _ | _ | _ | _ | _ | _ |
		
		// Player Rack:
		List<Tile> tiles = new List<Tile>();
		tiles.Add(new Tile(TileType.LETTER, 'R'));
		tiles.Add(new Tile(TileType.LETTER, 'H'));
		tiles.Add(new Tile(TileType.LETTER, 'T'));
		tiles.Add(new Tile(TileType.LETTER, 'H'));
		tiles.Add(new Tile(TileType.LETTER, 'M'));
		tiles.Add(new Tile(TileType.LETTER, 'S'));
		
		Debug.Log(board);
		Debug.Log("Player's rack contains: "  + tiles.ToArray());
		Coordinate coordinate = new Coordinate(0, 0);

		Prediction prediction = new Prediction(scoring);

//		long startTime = System.nanoTime();
		List<PredictionResult> list = prediction.predict(board, tiles, coordinate);
//		long endTime = System.nanoTime();
//		Debug.Log("Test results: " + ((endTime - startTime) / 1000000) + "ms.");
		foreach (PredictionResult result in list) {
			Debug.Log(result.score);
			Debug.Log(result.tiles.ToArray());
			Debug.Log(result.coordinates.ToArray());
		}
	}
	
	public static void standardTest() {
		

		ScrabbleScoringPolicy policy = new ScrabbleScoringPolicy();

		ScrabbleWordScoring wordScoring = new ScrabbleWordScoring(policy);

		Trie dict = new Trie();
		ScrabblePlayerMoveScoring scoring = new ScrabblePlayerMoveScoring(wordScoring, dict);

		ScrabbleBoard board = new ScrabbleBoard(ScrabbleBoardConfiguration.STANDARD);
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'H'), 3, 0);
		board.setTile(new Tile(TileType.LETTER, 'E'), 3, 1);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 2);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 3);
		board.setTile(new Tile(TileType.LETTER, 'O'), 3, 4);

		board.setTile(new Tile(TileType.LETTER, 'Y'), 0, 2);
		board.setTile(new Tile(TileType.LETTER, 'I'), 1, 2);
		board.setTile(new Tile(TileType.LETTER, 'E'), 2, 2);
		board.setTile(new Tile(TileType.LETTER, 'D'), 4, 2);

		// _ | _ | _ | H | _ |
		// _ | _ | _ | E | _ |
		// Y | I | E | L | D |
		// _ | _ | _ | L | _ |
		// _ | _ | _ | O | _ |
		
		// Player Rack:
		// D | A | R
		// Predictions should be DAY (for 7); RAY is less (only 6).
		List<Tile> tiles = new List<Tile>();
		tiles.Add(new Tile(TileType.LETTER, 'D'));
		tiles.Add(new Tile(TileType.LETTER, 'A'));
		tiles.Add(new Tile(TileType.LETTER, 'R'));
		
		Debug.Log(board);
		Debug.Log("Player's rack contains: "  + PredictionTests.stringify(tiles.ToArray()));
//		foreach (Tile t in tiles) {
//			Debug.Log (t);
//		}
		Coordinate coordinate = new Coordinate(0, 0);

		Prediction prediction = new Prediction(scoring);

//		long startTime = System.nanoTime();
//		Debug.Log("predicting");
		DateTime begin = DateTime.Now;

		List<PredictionResult> list = prediction.predict(board, tiles, coordinate);
//		Debug.Log("predicted");
		DateTime end = DateTime.Now;
		double ms = (end - begin).TotalMilliseconds;
		Debug.Log ("Prediction test for " + coordinate + " took " + ms + " ms.");

//		long endTime = System.nanoTime();
//		Debug.Log("Test results: " + ((endTime - startTime) / 1000000) + "ms.");

		foreach (PredictionResult result in list) {
			Debug.Log (PredictionTests.stringify(result.tiles.ToArray()) + " at " + PredictionTests.stringify(result.coordinates.ToArray()) + " for " + result.score );
		}
	}
	private static String stringify<T>(T[] array) {
		string s = "";
		foreach (T e in array) {
			s += e.ToString () + " ";
		}
		return s;
	}
	private static void microTest(Coordinate coordinate) {

		ScrabbleScoringPolicy policy = new ScrabbleScoringPolicy();

		ScrabbleWordScoring wordScoring = new ScrabbleWordScoring(policy);

		Trie dict = new Trie();
		ScrabblePlayerMoveScoring scoring = new ScrabblePlayerMoveScoring(wordScoring, dict);

		ScrabbleBoard board = new ScrabbleBoard(ScrabbleBoardConfiguration.MICRO);
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'A'), 0, 0);
		board.setTile(new Tile(TileType.LETTER, 'P'), 1, 0);
		board.setTile(new Tile(TileType.LETTER, 'E'), 2, 0);
		board.setTile(new Tile(TileType.LETTER, 'T'), 0, 2);
		board.setTile(new Tile(TileType.LETTER, 'A'), 1, 2);

		// A | P | E
		// _ | _ | _
		// T | A | _
		
		// Player Rack:
		// X | P
		// Predictions should be X; P is less score
		List<Tile> tiles = new List<Tile>();
		tiles.Add(new Tile(TileType.LETTER, 'X'));
		tiles.Add(new Tile(TileType.LETTER, 'P'));
		tiles.Add(new Tile(TileType.LETTER, 'E'));
		
		Prediction prediction = new Prediction(scoring);
		List<PredictionResult> resultSet = prediction.predict(board, tiles, coordinate);
		Debug.Log(board);
		Debug.Log("Player's rack contains: ");
		foreach (Tile t in tiles) {
			Debug.Log (t);
		}
		if(resultSet.Count == 0) {
			Debug.Log("There were no predictions for " + coordinate);
			return;
		}
		foreach (PredictionResult result in resultSet) {
			Debug.Log(result.score);
			Debug.Log(result.tiles.ToArray());
			Debug.Log(result.coordinates.ToArray());
			Debug.Log("");
		}
	}
}
