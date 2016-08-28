using System.Collections;
using System.Collections.Generic;
using System;

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
	public Solver(SolverConfiguration config, ScrabbleBoard board, ScrabblePlayerMoveScoring scoring) {
		this.config = config;
		this.board = board;
		this.scoring = scoring;
	}
	
	public List<PredictionResult> solve(List<Tile> tiles) {
		
		List<PredictionResult> resultSet = new List<PredictionResult>();
		for (int i = 0; i < board.dimension; i++) {

			for (int j = 0; j < board.dimension; j++) {

				Coordinate coordinate = new Coordinate(i, j);
				// TODO: spin up on new thread??
				Prediction prediction = new Prediction(scoring);
				foreach (PredictionResult result in prediction.predict(board, tiles, coordinate)) {
					resultSet.Add (result);
				}
			}
		}
		return top(resultSet);
	}
	private List<PredictionResult> top(List<PredictionResult> list) {
		list.Sort ();
		return list.GetRange(0, (list.Count < config.resultLimit) ? list.Count : config.resultLimit);
	}
}
