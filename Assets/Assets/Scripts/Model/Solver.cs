using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public interface SolverDelegate {
	MonoBehaviour mb ();
	void solutionDetermined(List<PredictionResult> predictions);
}

public sealed class SolverConfiguration {
	public int resultLimit;
	public SolverConfiguration(int resultLimit) {
		this.resultLimit = resultLimit;
	}
}

public sealed class Solver {
	
	private SolverConfiguration config;
	private ScrabbleBoard board;
	private ScrabblePlayerMoveScoring scoring;
	private SolverDelegate callback;
	public Solver(SolverConfiguration config, ScrabbleBoard board, ScrabblePlayerMoveScoring scoring, SolverDelegate callback) {
		this.config = config;
		this.board = board;
		this.scoring = scoring;
		this.callback = callback;
	}

	// coroutine version
	public void solve(List<Tile> tiles) {
		callback.mb().StartCoroutine (solveCoroutine(tiles));
	}

	public class IntRef {
		public int count = 0;
		public IntRef(int count) {
			this.count = count;
		}
		public void inc() {
			this.count += 1;
		}
	}

	public class PredictionCallback: PredictionDelegate {

		private List<PredictionResult> resultSet;
		private IntRef solvedPositions;
		private MonoBehaviour mono;

		public PredictionCallback(List<PredictionResult> resultSet, IntRef solvedPositions, MonoBehaviour mb) {
			this.resultSet = resultSet;
			this.solvedPositions = solvedPositions;
			this.mono = mb;
		}

		public MonoBehaviour mb () {
			return this.mono;
		}

		public void predictionsDetermined(List<PredictionResult> predictions) {
			Debug.Log ("A solution was discovered.");
			foreach (PredictionResult result in predictions) {
				resultSet.Add (result);
			}
			solvedPositions.inc ();
		}
	}

	private IntRef solvedPositions = new IntRef(0);
	private List<PredictionResult> resultSet = new List<PredictionResult>();
	private IEnumerator solveCoroutine(List<Tile> tiles) {


		for (int i = 0; i < board.dimension; i++) {
			
			for (int j = 0; j < board.dimension; j++) {


				Coordinate coordinate = new Coordinate(i, j);
				Debug.Log ("Attempting to solve the puzzle at (" + coordinate.x + ", " + coordinate.y + ").");

				yield return new WaitForSeconds(0.3f);

				Prediction prediction = new Prediction(scoring, new PredictionCallback(resultSet, solvedPositions, callback.mb()));
				prediction.predict (board, tiles, coordinate);

//				foreach (PredictionResult result in ) {
//					resultSet.Add (result);
//				}

			}
		}

		yield return new WaitUntil(() => solvedPositions.count == board.dimension * board.dimension);

		callback.solutionDetermined (top(resultSet));
	}

	// synchronous version
//	public List<PredictionResult> solve(List<Tile> tiles) {
//	
//		List<PredictionResult> resultSet = new List<PredictionResult>();
//		for (int i = 0; i < board.dimension; i++) {
//			
//			for (int j = 0; j < board.dimension; j++) {
//				
//				Coordinate coordinate = new Coordinate(i, j);
//				
//				Prediction prediction = new Prediction(scoring);
//				foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
//					resultSet.Add (result);
//				}
//			}
//		}
//		
//		return top(resultSet);
//	}
	private List<PredictionResult> top(List<PredictionResult> list) {
		list.Sort ();
		return list.GetRange(0, (list.Count < config.resultLimit) ? list.Count : config.resultLimit);
	}
	private List<PredictionResult> top(ThreadSafeList<PredictionResult> tslist) {
		List<PredictionResult> list = tslist.toList ();
		list.Sort ();
		return list.GetRange(0, (list.Count < config.resultLimit) ? list.Count : config.resultLimit);
	}
}


class ThreadSafeList<T> { 
	private List<T> _list = new List<T>();
	private object _sync = new object();
	public void Add(T value) {
		lock (_sync) {
			_list.Add(value);
		}
	}
	public List<T> toList() {
		return _list;
	}
}