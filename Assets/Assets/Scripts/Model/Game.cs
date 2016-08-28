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

	// void pass(Player player);
	// String define(String word);
	// void exchange(Player player, ArrayList<Tile> tiles);
}

public interface GameDelegate {

	void playerDrawTiles(Player player);
	void playersTurn(Player player);
	void predictionsDetermined(Player player, Coordinate coordinate, List<PredictionResult> predictions);
	void solutionDetermined(Player player, List<PredictionResult> predictions);
	void playerScored(Player player, int score);
	void playerWon(Player player);
	void scoreboardUpdated(Scoreboard scoreboard);
	void boardUpdated(ScrabbleBoard board);
}

public class ScrabbleGameConfiguration {
	public int predictions;
	public ScrabbleGameConfiguration(int predictions) {
		this.predictions = predictions;
	}
}



public sealed class Game: ScrabbleGame {

	// model objects
	private PlayerMoveBroker broker;
	private ScrabblePlayerMoveScoring scoring;

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
		ScrabbleScoringPolicy policy = new ScrabbleScoringPolicy();
		ScrabbleWordScoring wordScoring = new ScrabbleWordScoring(policy);

		Trie dict = new Trie();
		this.scoring = new ScrabblePlayerMoveScoring(wordScoring, dict);

		this.board = board;
		// previously placed tiles
		board.setTile(new Tile(TileType.LETTER, 'H'), 3, 0);
		board.setTile(new Tile(TileType.LETTER, 'E'), 3, 1);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 2);
		board.setTile(new Tile(TileType.LETTER, 'L'), 3, 3);
		board.setTile(new Tile(TileType.LETTER, 'O'), 3, 4);

		this.board = board;
		Debug.Log (board);
		// _ | _ | _ | H | _ |
		// _ | _ | _ | E | _ |
		// _ | _ | _ | L | _ |
		// _ | _ | _ | L | _ |
		// _ | _ | _ | O | _ |

		PlayerConfiguration playerconfig = new PlayerConfiguration(4);
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
			del.playerDrawTiles(player);
		}
		updateTurn();
	}

	public void predict(Player player, Coordinate coordinate) {
		if (turn == -1) return; // game didn't start

		Prediction prediction = new Prediction(scoring);
		del.predictionsDetermined(player, coordinate, prediction.predict(board, player.tiles, coordinate));
	}

	public void solve(Player player) {
		if (turn == -1) return; // game didn't start

		Solver s = new Solver(new SolverConfiguration(config.predictions), board, scoring);
		List<PredictionResult> solution = s.solve(player.tiles);
		del.solutionDetermined(player, solution);
	}

	public int play(Player player, AbstractPlayerMove move) {
		if (turn == -1) return -1; // game didn't start

		int score = broker.brokerMove(player, move);
		if (score == -1) {
			return -1; // illegal move
		}

		scoreboard.score(player, score);
		del.boardUpdated(board);
		del.playerScored(player, score);
		del.scoreboardUpdated(scoreboard);
		del.playerDrawTiles(player);

		if (gameOver()) {
			del.playerWon(player);
		}
		else {
			updateTurn();
		}

		return score;
	}

	private bool gameOver() {
		Solver s = new Solver(new SolverConfiguration(1), board, scoring);

		foreach (Player player in players) {
			if(s.solve(player.tiles).Count != 0) {
				return false;
			}
		}
		return true;
	}
}
