using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PredictionDelegate {
	MonoBehaviour mb ();
	void predictionsDetermined(List<PredictionResult> predictions);
}

public sealed class Prediction {

	private ScrabblePlayerMoveScoring scoring;
	private PredictionDelegate predictionDelegate;

	public Prediction(ScrabblePlayerMoveScoring scoring, PredictionDelegate predictionDelegate) {
		this.scoring = scoring;
		this.predictionDelegate = predictionDelegate;
	}

	public void predict(ScrabbleBoard board, List<Tile> tiles, Coordinate coordinate) {
		predictionDelegate.mb ().StartCoroutine (predictCoroutine (board, tiles, coordinate));
	}

	private IEnumerator predictCoroutine(ScrabbleBoard board, List<Tile> tiles, Coordinate coordinate) {
		List<Coordinate> horizontalHits = Raycasting.horizontalHitTest(board, coordinate);
		List<Coordinate> verticalHits = Raycasting.verticalHitTest(board, coordinate);
		List<PredictionResult> predictions = new List<PredictionResult>();

		List<List<Tile>> pset = PowerSet.powerset(tiles);
		foreach (List<Tile> s in pset) {

			Permutator<Tile> permutator = new Permutator<Tile>(s.ToArray());
			Tile[] permutation = null;
			while (true) {
				permutation = permutator.next();
				if (permutation == null) break;
				if (permutation.Length == 0) continue;
				if (Raycasting.horizontalContains (horizontalHits, coordinate, permutation.Length + horizontalHits.Count)) {
					PredictionResult horizontalPrediction = predict (board, permutation, coordinate, ScrabbleScoringDirection.HORIZONTAL); 
					if (horizontalPrediction.score != -1) {
						predictions.Add (horizontalPrediction);
					}
				}
				if (Raycasting.verticalContains (verticalHits, coordinate, permutation.Length + verticalHits.Count)) {
					PredictionResult verticalPrediction = predict (board, permutation, coordinate, ScrabbleScoringDirection.VERTICAL); 
					if (verticalPrediction.score != -1) { 
						predictions.Add (verticalPrediction);
					}
				}
			}

			yield return new WaitForEndOfFrame();


		}
		predictionDelegate.predictionsDetermined (predictions);
	}

	// synchronous way
//	public List<PredictionResult> predict(ScrabbleBoard board, List<Tile> tiles, Coordinate coordinate) {
//
//		//System.out.println("Predicting " + Arrays.toString(tiles.toArray()) + " to be placed on board \n" + board + "\n at coordinate: " + coordinate);
//
//		// predicted moves much touch at least one fixed tile. (TODO: except for the first move)
//		List<Coordinate> horizontalHits = Raycasting.horizontalHitTest(board, coordinate);
//		List<Coordinate> verticalHits = Raycasting.verticalHitTest(board, coordinate);
//
////		Debug.Log ("h_hits: " + horizontalHits.Count);
////		Debug.Log ("v_hits: " + verticalHits.Count);
//
//		List<PredictionResult> predictions = new List<PredictionResult>();
//
//		List<List<Tile>> pset = PowerSet.powerset(tiles);
//		foreach (List<Tile> s in pset) {
//			Permutator<Tile> permutator = new Permutator<Tile>(s.ToArray());
//			Tile[] permutation = null;
//			while (true) {
//				permutation = permutator.next();
//				if (permutation == null) break;
//				if (permutation.Length == 0) continue;
//
//				/*
//				Debug.Log("Analyzing permutation: ");
//				foreach (Tile t in permutation) {
//					Debug.Log(t + "|");
//				}
//				Debug.Log("");
//				*/
//
////				Debug.Log ("Checking horizontal");
//				if (Raycasting.horizontalContains (horizontalHits, coordinate, permutation.Length + horizontalHits.Count)) {
//					PredictionResult horizontalPrediction = predict (board, permutation, coordinate, ScrabbleScoringDirection.HORIZONTAL); 
//					if (horizontalPrediction.score != -1) {
//						predictions.Add (horizontalPrediction);
//					}
//				} /*else {
//					Debug.Log ("nada");
//				}*/
//
////				Debug.Log ("Checking vertical for coordinate: " + coordinate);
////				Debug.Log ("hits: " + verticalHits);
////				foreach (Coordinate c in verticalHits) {
////					Debug.Log (c + " ... ");
////				}
////				Debug.Log ("");
////				Debug.Log ("radius: " + (permutation.Length + verticalHits.Count));
////				Debug.Log ("Checking vertical");
//				if (Raycasting.verticalContains (verticalHits, coordinate, permutation.Length + verticalHits.Count)) {
//					PredictionResult verticalPrediction = predict (board, permutation, coordinate, ScrabbleScoringDirection.VERTICAL); 
//					if (verticalPrediction.score != -1) { 
//						predictions.Add (verticalPrediction);
//					}
//				} /* else {
//					Debug.Log ("nada");
//				}*/
//			}
//		}
//		return predictions;
//	}

	private PredictionResult predict(ScrabbleBoard board, Tile[] permutation, Coordinate coordinate, ScrabbleScoringDirection direction) {

		int dimension = board.dimension;
		Tile[,] context = new Tile[dimension,dimension];
		List<Coordinate> placements = new List<Coordinate>();
		Coordinate placement = coordinate;

//		Debug.Log ("Perm size: " + permutation.Length);


		foreach (Tile tile in permutation) {
//			Debug.Log ("Tile in permutation: " + tile);
			placement = place(board, context, tile, placement, direction);
			if (placement != null) {
				placements.Add (placement);
			} else {
				break;
			}
		}
		if (placements.Count == permutation.Length) { 
			PredictedBoard prediction = new PredictedBoard(board, new PredictedContext(context));
			String root = "";
			int score = scoring.score(prediction, placements.ToArray(), direction, out root);

//			Debug.Log ("board: \n" + prediction);
//			Debug.Log ("score: " + score);
			return new PredictionResult(permutation, placements.ToArray(), score, root);
		}
		else {
			//			Debug.Log("the sizes did not match..." + placements.Count + " " + permutation.Length);
			return new PredictionResult(new Tile[0], new Coordinate[0], -1, "");
		}
	}
	private Coordinate place(ScrabbleBoard fixedBoard, Tile[,] context, Tile tile, Coordinate origin, ScrabbleScoringDirection direction) {

//		Debug.Log("place() invoked for " + origin);
		TileSpace space = fixedBoard.getSpace(origin.x, origin.y);
		Tile placement = null;
		Tile fixedTile = null;

		Coordinate curr = origin;
		while(space != null) { // space == null means out of bounds
//			Debug.Log("Attempting to place " + tile + " at " + curr);
			placement = context[curr.x,curr.y];
			fixedTile = fixedBoard.getTile(curr.x, curr.y);
			if (placement != null || fixedTile != null) { // already predicted/placed a letter here
				/*
				if (fixedTile != null) {
					Debug.Log("there was a fixed letter at: " + curr);
				}
				if (placement != null) {
					Debug.Log("there was a predicted letter at: " + curr);
				}*/
				curr = new Coordinate(curr.x + ScrabbleScoringDirectionHelper.horizontalDelta(direction), curr.y + ScrabbleScoringDirectionHelper.verticalDelta(direction));
				space = fixedBoard.getSpace(curr.x, curr.y);
				continue;
			}

//			Debug.Log("Placing " + tile.getLetter() + " at " + curr);
			context[curr.x,curr.y] = tile;
			return curr;
		}
//		Debug.Log("Out of bounds at: " + curr);
		return null;
	}
}


