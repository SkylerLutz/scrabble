using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//public class Game {
//	public Game() {
//
//	}
//	public int play(Player player, AbstractPlayerMove move) {
//		return -1;
//	}
//}


public interface ScrabbleGame {
	void start();
	void predict(Player player, Coordinate coordinate);
	void solve(Player player);
	int play(Player player, AbstractPlayerMove move);
	int valueOf (Tile tile);
	// void pass(Player player);
	// String define(String word);
	// void exchange(Player player, ArrayList<Tile> tiles);
}

public interface GameDelegate {

	void playerDrewTiles(Player player, Tile[] tiles);
	void playersTurn(Player player);
	void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions);
	void solutionDetermined(Player player, List<PredictionResult> predictions);
	void playerScored(Player player, int score);
	void playerWon(Player player);
	void scoreboardUpdated(Scoreboard scoreboard);
}

public class ScrabbleGameConfiguration {
	public int predictions;
	public PlayerConfiguration playerconfig;
	public ScrabbleGameConfiguration(int predictions, PlayerConfiguration playerconfig) {
		this.predictions = predictions;
		this.playerconfig = playerconfig;
	}
}



public sealed class Game: ScrabbleGame {

	// model objects
	private PlayerMoveBroker broker;
	private ScrabblePlayerMoveScoring scoring;
	private ScrabbleScoringPolicy letterScoring;

	// game objects
	private Player[] players;
	private Scoreboard scoreboard;
	public ScrabbleBoard board;

	private ScrabbleGameConfiguration config;
	private GameDelegate del;

	
	public Game(ScrabbleGameConfiguration config, ScrabbleBoard board, Player[] players, GameDelegate del) {
		this.config = config;
		this.del = del;
		this.players = players;
		this.scoreboard = new Scoreboard(players);
		this.letterScoring = new ScrabbleScoringPolicy();
		ScrabbleWordScoring wordScoring = new ScrabbleWordScoring(letterScoring);

		Trie dict = new Trie();
		this.scoring = new ScrabblePlayerMoveScoring(wordScoring, dict);

		this.board = board;

		PlayerConfiguration playerconfig = config.playerconfig;
		Bag bag = new Bag(BagConfiguration.STANDARD_NO_WILDCARDS);
		PlayerMoveBroker broker = new PlayerMoveBroker(scoring, board, playerconfig, bag);
		this.broker = broker;
	}

	private int turn = -1;
	private void updateTurn() {
		turn += 1;
		Player active = players[turn % players.Length];
		del.playersTurn(active);
	}

	public void start() {

		// give players 7 tiles
		foreach (Player player in players) {
			broker.replenish(player);
			del.playerDrewTiles(player, player.tiles.ToArray());
		}
		updateTurn();
	}

	public void predict(Player player, Coordinate coordinate) {
		if (turn == -1) return; // game didn't start

//		Prediction prediction = new Prediction(scoring);
//		del.predictionsDetermined(player, coordinate, prediction.predict(board, player.tiles, coordinate));
	}

	public class SolverCallback: SolverDelegate {
		
		private Player player;
		private GameDelegate gameDelegate;

		public SolverCallback(Player player, GameDelegate gameDelegate) {
			this.player = player;
			this.gameDelegate = gameDelegate;
		}

		public void solutionDetermined(List<PredictionResult> predictions) {
			gameDelegate.solutionDetermined (player, predictions);
		}
		public MonoBehaviour mb() {
			return (MonoBehaviour)gameDelegate;
		}
	}

	public void solve(Player player) {
		if (turn == -1) return; // game didn't start

		Solver s = new Solver(new SolverConfiguration(config.predictions), board, scoring, new SolverCallback(player, del));
		s.solve(player.tiles); // delegate will be invoked
	}

	public int play(Player player, AbstractPlayerMove move) {
		if (turn == -1) return -1; // game didn't start

		HashSet<Tile> originalTiles = new HashSet<Tile> (player.tiles);
		Debug.Log ("original: ");
		foreach (Tile t in originalTiles) {
			Debug.Log (t);
		}
		int score = broker.brokerMove(player, move);
		if (score == -1) {
			return -1; // illegal move
		}

		HashSet<Tile> current = new HashSet<Tile> (player.tiles);
		Debug.Log ("current state: ");
		foreach (Tile t in current) {
			Debug.Log (t);
		}
		current.ExceptWith (originalTiles);
		Debug.Log ("diff: ");
		foreach (Tile t in current) {
			Debug.Log (t);
		}

		List<Tile> drawn = new List<Tile> (current);
		scoreboard.score(player, score);
		del.playerScored(player, score);
		del.scoreboardUpdated(scoreboard);
		del.playerDrewTiles(player, drawn.ToArray());



		// TODO: check gameover
//		if (gameOver()) {
//			del.playerWon(player);
//		}
//		else {
//			updateTurn();
//		}
		updateTurn();
		return score;
	}

	public int valueOf (Tile tile) {
		return letterScoring.valueOf (tile.getLetter ());
	}

	private bool gameOver() {
//		Solver s = new Solver(new SolverConfiguration(1), board, scoring);
//
//		foreach (Player player in players) {
//			if(s.solve(player.tiles).Count != 0) {
//				return false;
//			}
//		}
		return false;
	}
}
